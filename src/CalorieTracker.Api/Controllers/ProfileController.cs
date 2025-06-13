using AutoMapper;
using CalorieTracker.Api.Models.Profile;
using CalorieTracker.Domain.Entities;
using CalorieTracker.Domain.Services;
using CalorieTracker.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CalorieTracker.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
	// ---------- GET PROFILE ----------
	/// <summary>
	/// zwraca profil aktualnie zalogowanego użytkownika
	/// jeśli profil nie istnieje zwraca 204 NoContent
	/// </summary>
	[HttpGet]
	public async Task<ActionResult<UserProfileDto>> GetProfile(
		[FromServices] AppDbContext db,
		[FromServices] UserManager<ApplicationUser> users)
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (userId is null) return Unauthorized();

		var user = await users.FindByIdAsync(userId);
		if (user is null) return Unauthorized();

		var profile = await db.UserProfiles
			.FirstAsync(p => p.UserId == userId);

		// obliczanie flagi kompletności
		bool isComplete =
			profile.Age is not null &&
			profile.HeightCm is not null &&
			profile.WeightKg is not null &&
			profile.TargetWeightKg is not null &&
			profile.WeeklyGoalChangeKg is not null &&
			profile.Gender is not null &&
			profile.ActivityLevel is not null &&
			profile.Goal is not null &&
			profile.MealPlan is not null &&
			profile.MealPlan.Length == 6 &&
			profile.MealPlan.Any(x => x);

		// scala dane z ApplicationUser + UserProfile
		var dto = new UserProfileDto
		{
			Email = user.Email!,
			FirstName = user.FirstName,
			LastName = user.LastName,
			Goal = profile?.Goal?.ToString(),
			Gender = profile?.Gender?.ToString(),
			Age = profile?.Age,
			HeightCm = profile?.HeightCm,
			WeightKg = profile?.WeightKg,
			TargetWeightKg = profile?.TargetWeightKg,
			ActivityLevel = profile?.ActivityLevel?.ToString(),
			WeeklyGoalChangeKg = profile?.WeeklyGoalChangeKg,
			MealPlan = profile?.MealPlan?.ToList(),
			IsComplete = isComplete
		};

		return Ok(dto);
	}

	// ---------- PUT PROFILE ----------
	/// <summary>
	/// Tworzy lub aktualizuje profil użytkownika.
	/// Automatycznie tworzy pomiar masy ciała jeśli waga się zmieniła.
	/// </summary>
	[HttpPut]
	public async Task<IActionResult> UpdateProfile(
		[FromBody] UpdateUserProfileRequest req,
		[FromServices] AppDbContext db,
		[FromServices] UserManager<ApplicationUser> users,
		[FromServices] IMapper mapper,
		[FromServices] WeightAnalysisService weightAnalysis)
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (userId is null) return Unauthorized();

		// aktualizacja danych konta 
		var user = await users.FindByIdAsync(userId);
		if (user is null) return Unauthorized();

		user.FirstName = req.FirstName;
		user.LastName = req.LastName;
		await users.UpdateAsync(user);

		// aktualizacja danych profilu
		var profile = await db.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);

		bool isNewProfile = profile is null;
		float? oldWeight = profile?.WeightKg;

		if (profile is null)
		{
			profile = mapper.Map<UserProfile>(req);
			profile.UserId = userId;
			db.UserProfiles.Add(profile);
		}
		else
		{
			mapper.Map(req, profile);
		}

		await db.SaveChangesAsync();

		// jeśli waga się zmieniła
		if (req.WeightKg.HasValue && (isNewProfile || oldWeight != req.WeightKg.Value))
		{
			// Sprawdza czy nie ma już pomiaru na dzisiaj
			var today = DateOnly.FromDateTime(DateTime.Today);
			var existingMeasurement = await db.WeightMeasurements
				.FirstOrDefaultAsync(w => w.UserId == userId && w.MeasurementDate == today);

			if (existingMeasurement == null)
			{
				// Pobiera poprzedni pomiar dla kalkulacji zmiany
				var previousMeasurement = await db.WeightMeasurements
					.Where(w => w.UserId == userId && w.MeasurementDate < today)
					.OrderByDescending(w => w.MeasurementDate)
					.FirstOrDefaultAsync();

				// Twrzy nowy pomiar
				var measurement = new WeightMeasurement
				{
					UserId = userId,
					MeasurementDate = today,
					WeightKg = req.WeightKg.Value,
					CreatedAt = DateTime.UtcNow,
					UpdatedAt = DateTime.UtcNow
				};

				// Wypełnia kalkulowane pola
				weightAnalysis.FillCalculatedFields(measurement, profile, previousMeasurement);

				db.WeightMeasurements.Add(measurement);
				await db.SaveChangesAsync();
			}
			else
			{
				// Aktualizuje istniejący pomiar na dzisiaj
				existingMeasurement.WeightKg = req.WeightKg.Value;
				existingMeasurement.UpdatedAt = DateTime.UtcNow;

				// Pobiera poprzedni pomiar dla kalkulacji zmiany
				var previousMeasurement = await db.WeightMeasurements
					.Where(w => w.UserId == userId && w.MeasurementDate < today)
					.OrderByDescending(w => w.MeasurementDate)
					.FirstOrDefaultAsync();

				// Przelicza kalkulowane pola
				weightAnalysis.FillCalculatedFields(existingMeasurement, profile, previousMeasurement);

				await db.SaveChangesAsync();
			}
		}

		return NoContent();
	}
}