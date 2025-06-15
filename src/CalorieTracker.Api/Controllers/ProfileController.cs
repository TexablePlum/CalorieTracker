// Plik ProfileController.cs - kontroler zarządzania profilami użytkowników w aplikacji CalorieTracker.
// Odpowiada za pobieranie i aktualizację profili użytkowników oraz automatyczne zarządzanie pomiarami masy ciała.

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

/// <summary>
/// Kontroler profili użytkowników zarządzający danymi osobistymi i preferencjami dietetycznymi.
/// Obsługuje pobieranie i aktualizację profili użytkowników z automatyczną integracją pomiarów masy ciała.
/// Wymaga autoryzacji dla wszystkich operacji i implementuje logikę biznesową związaną z zarządzaniem wagą.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
	/// <summary>
	/// Pobiera kompletny profil aktualnie zalogowanego użytkownika.
	/// Łączy dane z tabeli ApplicationUser (konto) i UserProfile (preferencje dietetyczne).
	/// Automatycznie oblicza flagę kompletności profilu na podstawie wypełnionych pól.
	/// </summary>
	/// <param name="db">Kontekst bazy danych (wstrzykiwany przez DI).</param>
	/// <param name="users">Manager użytkowników Identity (wstrzykiwany przez DI).</param>
	/// <returns>
	/// 200 OK z danymi profilu - gdy profil użytkownika istnieje i został pomyślnie pobrany.
	/// 401 Unauthorized - gdy użytkownik nie jest zalogowany lub jego konto nie istnieje.
	/// </returns>
	[HttpGet]
	public async Task<ActionResult<UserProfileDto>> GetProfile(
		[FromServices] AppDbContext db,
		[FromServices] UserManager<ApplicationUser> users)
	{
		// Ekstraktuje ID użytkownika z claims tokenu JWT
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (userId is null) return Unauthorized();

		// Pobiera dane konta użytkownika z systemu Identity
		var user = await users.FindByIdAsync(userId);
		if (user is null) return Unauthorized();

		// Pobiera profil użytkownika z bazy danych
		var profile = await db.UserProfiles
			   .FirstAsync(p => p.UserId == userId);

		// Oblicza flagę kompletności profilu sprawdzając wszystkie wymagane pola
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
			   profile.MealPlan.Any(x => x); // Przynajmniej jeden posiłek musi być wybrany

		// Scala dane z obu tabel w jeden DTO dla frontendu
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

	/// <summary>
	/// Tworzy nowy lub aktualizuje istniejący profil użytkownika.
	/// Automatycznie zarządza pomiarami masy ciała - tworzy nowy pomiar gdy waga się zmienia.
	/// Implementuje inteligentną logikę aktualizacji pomiarów z obsługą przypadków granicznych.
	/// </summary>
	/// <param name="req">Dane profilu do aktualizacji zawierające informacje osobiste i preferencje dietetyczne.</param>
	/// <param name="db">Kontekst bazy danych (wstrzykiwany przez DI).</param>
	/// <param name="users">Manager użytkowników Identity (wstrzykiwany przez DI).</param>
	/// <param name="mapper">Mapper AutoMapper (wstrzykiwany przez DI).</param>
	/// <param name="weightAnalysis">Serwis analizy masy ciała do kalkulacji BMI i postępu (wstrzykiwany przez DI).</param>
	/// <returns>
	/// 204 NoContent - gdy aktualizacja profilu przebiegła pomyślnie.
	/// 400 BadRequest - gdy dane profilu są nieprawidłowe lub nie przechodzą walidacji.
	/// 401 Unauthorized - gdy użytkownik nie jest zalogowany lub jego konto nie istnieje.
	/// </returns>
	[HttpPut]
	public async Task<IActionResult> UpdateProfile(
		[FromBody] UpdateUserProfileRequest req,
		[FromServices] AppDbContext db,
		[FromServices] UserManager<ApplicationUser> users,
		[FromServices] IMapper mapper,
		[FromServices] WeightAnalysisService weightAnalysis)
	{
		// Weryfikuje autoryzację użytkownika
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (userId is null) return Unauthorized();

		// Aktualizuje podstawowe dane konta użytkownika (imię, nazwisko)
		var user = await users.FindByIdAsync(userId);
		if (user is null) return Unauthorized();

		user.FirstName = req.FirstName;
		user.LastName = req.LastName;
		await users.UpdateAsync(user);

		// Pobiera istniejący profil lub przygotowuje się do utworzenia nowego
		var profile = await db.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);

		bool isNewProfile = profile is null;
		float? oldWeight = profile?.WeightKg; // Zachowuje poprzednią wagę do porównania

		// Tworzy nowy profil lub aktualizuje istniejący
		if (profile is null)
		{
			// Tworzy nowy profil przy pierwszej aktualizacji
			profile = mapper.Map<UserProfile>(req);
			profile.UserId = userId;
			db.UserProfiles.Add(profile);
		}
		else
		{
			// Aktualizuje istniejący profil zachowując ID i relacje
			mapper.Map(req, profile);
		}

		await db.SaveChangesAsync();

		// Automatyczne zarządzanie pomiarami masy ciała przy zmianie wagi
		if (req.WeightKg.HasValue && (isNewProfile || oldWeight != req.WeightKg.Value))
		{
			var today = DateOnly.FromDateTime(DateTime.Today);

			// Sprawdza czy już istnieje pomiar na dzisiaj
			var existingMeasurement = await db.WeightMeasurements
				   .FirstOrDefaultAsync(w => w.UserId == userId && w.MeasurementDate == today);

			if (existingMeasurement == null)
			{
				// Tworzy nowy pomiar masy ciała

				// Pobiera poprzedni pomiar dla kalkulacji zmiany względnej
				var previousMeasurement = await db.WeightMeasurements
					   .Where(w => w.UserId == userId && w.MeasurementDate < today)
					   .OrderByDescending(w => w.MeasurementDate)
					   .FirstOrDefaultAsync();

				// Tworzy nowy rekord pomiaru
				var measurement = new WeightMeasurement
				{
					UserId = userId,
					MeasurementDate = today,
					WeightKg = req.WeightKg.Value,
					CreatedAt = DateTime.UtcNow,
					UpdatedAt = DateTime.UtcNow
				};

				// Automatycznie wypełnia pola kalkulowane (BMI, postęp względem celu)
				weightAnalysis.FillCalculatedFields(measurement, profile, previousMeasurement);

				db.WeightMeasurements.Add(measurement);
				await db.SaveChangesAsync();
			}
			else
			{
				// Aktualizuje istniejący pomiar z dzisiaj

				existingMeasurement.WeightKg = req.WeightKg.Value;
				existingMeasurement.UpdatedAt = DateTime.UtcNow;

				// Pobiera poprzedni pomiar dla ponownych kalkulacji
				var previousMeasurement = await db.WeightMeasurements
					   .Where(w => w.UserId == userId && w.MeasurementDate < today)
					   .OrderByDescending(w => w.MeasurementDate)
					   .FirstOrDefaultAsync();

				// Przelicza wszystkie pola kalkulowane z nowymi wartościami
				weightAnalysis.FillCalculatedFields(existingMeasurement, profile, previousMeasurement);

				await db.SaveChangesAsync();
			}
		}

		return NoContent();
	}
}