using AutoMapper;
using CalorieTracker.Api.Models.Profile;
using CalorieTracker.Domain.Entities;
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
	/// </summary>
	[HttpPut]
	public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileRequest req, [FromServices] AppDbContext db, [FromServices] UserManager<ApplicationUser> users, [FromServices] IMapper mapper)
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
		return NoContent();
	}

}