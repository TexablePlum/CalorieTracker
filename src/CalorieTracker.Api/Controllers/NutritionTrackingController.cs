// Plik NutritionTrackingController.cs - kontroler śledzenia żywienia aplikacji CalorieTracker.
// Odpowiada za udostępnianie endpointów związanych z logowaniem posiłków i monitorowaniem postępu żywieniowego.

using AutoMapper;
using CalorieTracker.Api.Attributes;
using CalorieTracker.Api.Models.NutritionTracking;
using CalorieTracker.Application.NutritionTracking.Commands;
using CalorieTracker.Application.NutritionTracking.Handlers;
using CalorieTracker.Application.NutritionTracking.Queries;
using CalorieTracker.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CalorieTracker.Api.Controllers
{
	/// <summary>
	/// Kontroler śledzenia żywienia zarządzający logowaniem posiłków i monitorowaniem postępu żywieniowego.
	/// Udostępnia endpointy do CRUD posiłków, pobierania dziennego postępu oraz analizy historii żywieniowej.
	/// Wymaga autoryzacji oraz kompletnego profilu użytkownika dla wszystkich operacji.
	/// Implementuje wzorzec CQRS z dedykowanymi handlerami dla każdej operacji biznesowej.
	/// </summary>
	[Authorize]
	[RequireCompleteProfile]
	[ApiController]
	[Route("api/nutrition-tracking")]
	public class NutritionTrackingController : ControllerBase
	{
		/// <summary>
		/// Mapper AutoMapper do konwersji między Value Objects/Entities a modelami DTO.
		/// </summary>
		private readonly IMapper _mapper;

		/// <summary>
		/// Handler odpowiedzialny za dodawanie nowych posiłków do dziennika.
		/// </summary>
		private readonly LogMealHandler _logMealHandler;

		/// <summary>
		/// Handler odpowiedzialny za aktualizację wpisów posiłków.
		/// </summary>
		private readonly UpdateMealLogHandler _updateMealLogHandler;

		/// <summary>
		/// Handler odpowiedzialny za usuwanie wpisów posiłków.
		/// </summary>
		private readonly DeleteMealLogHandler _deleteMealLogHandler;

		/// <summary>
		/// Handler odpowiedzialny za pobieranie dziennego postępu żywieniowego.
		/// </summary>
		private readonly GetDailyNutritionProgressHandler _getDailyProgressHandler;

		/// <summary>
		/// Handler odpowiedzialny za pobieranie historii posiłków.
		/// </summary>
		private readonly GetMealHistoryHandler _getMealHistoryHandler;

		/// <summary>
		/// Handler odpowiedzialny za dodawanie wpisów spożycia wody.
		/// </summary>
		private readonly LogWaterIntakeHandler _logWaterIntakeHandler;

		/// <summary>
		/// Handler odpowiedzialny za aktualizację wpisów spożycia wody.
		/// </summary>
		private readonly UpdateWaterIntakeHandler _updateWaterIntakeHandler;

		/// <summary>
		/// Handler odpowiedzialny za usuwanie wpisów spożycia wody.
		/// </summary>
		private readonly DeleteWaterIntakeHandler _deleteWaterIntakeHandler;

		/// <summary>
		/// Inicjalizuje nową instancję kontrolera nutrition tracking z wymaganymi zależnościami.
		/// </summary>
		/// <param name="mapper">Mapper AutoMapper do transformacji obiektów.</param>
		/// <param name="logMealHandler">Handler dodawania posiłków.</param>
		/// <param name="updateMealLogHandler">Handler aktualizacji posiłków.</param>
		/// <param name="deleteMealLogHandler">Handler usuwania posiłków.</param>
		/// <param name="getDailyProgressHandler">Handler dziennego postępu.</param>
		/// <param name="getMealHistoryHandler">Handler historii posiłków.</param>
		/// <param name="logWaterIntakeHandler">Handler logowania spożycia wody.</param>
		/// <param name="deleteWaterIntakeHandler">Handler usuwania spożycia wody.</param>
		/// <param name="updateWaterIntakeHandler">Handler aktualizacji spożycia wody.</param>
		public NutritionTrackingController(
			IMapper mapper,
			LogMealHandler logMealHandler,
			UpdateMealLogHandler updateMealLogHandler,
			DeleteMealLogHandler deleteMealLogHandler,
			GetDailyNutritionProgressHandler getDailyProgressHandler,
			GetMealHistoryHandler getMealHistoryHandler,
			LogWaterIntakeHandler logWaterIntakeHandler,
			UpdateWaterIntakeHandler updateWaterIntakeHandler,
			DeleteWaterIntakeHandler deleteWaterIntakeHandler)
		{
			_mapper = mapper;
			_logMealHandler = logMealHandler;
			_updateMealLogHandler = updateMealLogHandler;
			_deleteMealLogHandler = deleteMealLogHandler;
			_getDailyProgressHandler = getDailyProgressHandler;
			_getMealHistoryHandler = getMealHistoryHandler;
			_logWaterIntakeHandler = logWaterIntakeHandler;
			_updateWaterIntakeHandler = updateWaterIntakeHandler;
			_deleteWaterIntakeHandler = deleteWaterIntakeHandler;
		}

		/// <summary>
		/// Dodaje nowy posiłek do dziennika żywieniowego użytkownika.
		/// Automatycznie kalkuluje wartości odżywcze na podstawie produktu lub przepisu.
		/// Obsługuje zarówno produkty spożywcze jak i przepisy kulinarne.
		/// </summary>
		/// <param name="request">Dane nowego posiłku do zalogowania.</param>
		/// <returns>
		/// 201 Created z ID utworzonego wpisu - posiłek został pomyślnie zalogowany.
		/// 400 BadRequest - nieprawidłowe dane wejściowe (brakuje ProductId/RecipeId, nieprawidłowa ilość, etc.).
		/// 401 Unauthorized - użytkownik nie jest zalogowany.
		/// 403 Forbidden - profil użytkownika nie jest kompletny.
		/// 404 NotFound - podany produkt lub przepis nie istnieje.
		/// </returns>
		[HttpPost("log-meal")]
		public async Task<ActionResult<object>> LogMeal([FromBody] LogMealRequest request)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId is null)
				return Unauthorized("Brak identyfikatora użytkownika w tokenie");

			try
			{
				// Mapuje request na komendę
				var command = _mapper.Map<LogMealCommand>(request);
				command.UserId = userId;

				// Wykonuje komendę przez handler
				var mealLogEntryId = await _logMealHandler.Handle(command);

				// Zwraca sukces
				return Created($"/api/nutrition-tracking/meals/{mealLogEntryId}", new
				{
					Id = mealLogEntryId,
					Message = "Posiłek został pomyślnie zalogowany"
				});
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (InvalidOperationException ex)
			{
				return NotFound(ex.Message);
			}
		}

		/// <summary>
		/// Pobiera kompletny dzienny postęp żywieniowy użytkownika.
		/// Zwraca cele, rzeczywiste spożycie, pozostałe potrzeby, procenty postępu oraz listę posiłków dnia.
		/// To jest główny endpoint aplikacji - zastępuje poprzednie /nutrition/daily-requirements.
		/// </summary>
		/// <param name="date">Data w formacie yyyy-MM-dd (opcjonalnie - domyślnie dzisiaj).</param>
		/// <returns>
		/// 200 OK z kompletnym postępem żywieniowym - cele, spożycie, pozostałe, procenty, posiłki.
		/// 400 BadRequest - nieprawidłowy format daty lub niekompletny profil użytkownika.
		/// 401 Unauthorized - użytkownik nie jest zalogowany.
		/// 403 Forbidden - profil użytkownika nie jest kompletny.
		/// 404 NotFound - profil użytkownika nie istnieje w systemie.
		/// </returns>
		[HttpGet("daily-progress")]
		public async Task<ActionResult<DailyNutritionProgressDto>> GetDailyProgress([FromQuery] string? date = null)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId is null)
				return Unauthorized("Brak identyfikatora użytkownika w tokenie");

			try
			{
				// Parsuje opcjonalne daty
				DateOnly? targetDate = null;
				if (!string.IsNullOrEmpty(date))
				{
					if (!DateOnly.TryParse(date, out var parsedDate))
						return BadRequest("Nieprawidłowy format daty. Użyj yyyy-MM-dd.");
					targetDate = parsedDate;
				}

				// Tworzy zapytanie i wykonuje
				var query = new GetDailyNutritionProgressQuery(userId, targetDate);
				var dailyProgress = await _getDailyProgressHandler.Handle(query);

				if (dailyProgress == null)
					return NotFound("Profil użytkownika nie został znaleziony");

				// Mapuje Value Object na DTO
				var dto = _mapper.Map<DailyNutritionProgressDto>(dailyProgress);
				return Ok(dto);
			}
			catch (ArgumentException ex)
			{
				return BadRequest($"Błąd danych profilu: {ex.Message}");
			}
		}

		/// <summary>
		/// Pobiera listę posiłków użytkownika z konkretnego dnia.
		/// Zwraca szczegółowe informacje o wszystkich posiłkach zalogowanych w danym dniu.
		/// </summary>
		/// <param name="date">Data w formacie yyyy-MM-dd (opcjonalnie - domyślnie dzisiaj).</param>
		/// <returns>
		/// 200 OK z listą posiłków z danego dnia.
		/// 400 BadRequest - nieprawidłowy format daty.
		/// 401 Unauthorized - użytkownik nie jest zalogowany.
		/// 403 Forbidden - profil użytkownika nie jest kompletny.
		/// </returns>
		[HttpGet("meal-history")]
		public async Task<ActionResult<List<MealLogEntryDto>>> GetMealHistory([FromQuery] string? date = null)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId is null)
				return Unauthorized("Brak identyfikatora użytkownika w tokenie");

			try
			{
				// Parsuje opcjonalną datę
				DateOnly targetDate = DateOnly.FromDateTime(DateTime.Now); // domyślnie dzisiaj
				if (!string.IsNullOrEmpty(date))
				{
					if (!DateOnly.TryParse(date, out var parsedDate))
						return BadRequest("Nieprawidłowy format daty. Użyj yyyy-MM-dd.");
					targetDate = parsedDate;
				}

				// Pobiera posiłki przez handler
				var meals = await _getMealHistoryHandler.GetMealsForDay(userId, targetDate);

				// Mapuje na DTO
				var mealDtos = _mapper.Map<List<MealLogEntryDto>>(meals);

				return Ok(mealDtos);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (Exception)
			{
				return StatusCode(500, "Wystąpił błąd podczas pobierania posiłków");
			}
		}

		/// <summary>
		/// Aktualizuje istniejący wpis posiłku w dzienniku użytkownika.
		/// Pozwala na zmianę ilości, typu posiłku, czasu spożycia oraz notatek.
		/// Automatycznie przelicza wartości odżywcze dla nowej ilości.
		/// </summary>
		/// <param name="id">Identyfikator wpisu posiłku do aktualizacji.</param>
		/// <param name="request">Nowe dane wpisu posiłku.</param>
		/// <returns>
		/// 200 OK - wpis został pomyślnie zaktualizowany.
		/// 400 BadRequest - nieprawidłowe dane wejściowe.
		/// 401 Unauthorized - użytkownik nie jest zalogowany.
		/// 403 Forbidden - profil użytkownika nie jest kompletny.
		/// 404 NotFound - wpis posiłku nie istnieje lub nie należy do użytkownika.
		/// </returns>
		[HttpPut("meals/{id:guid}")]
		public async Task<ActionResult> UpdateMealLog(Guid id, [FromBody] UpdateMealLogRequest request)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId is null)
				return Unauthorized("Brak identyfikatora użytkownika w tokenie");

			try
			{
				// Mapuje request na komendę
				var command = _mapper.Map<UpdateMealLogCommand>(request);
				command.MealLogEntryId = id;
				command.UserId = userId;

				// Wykonuje aktualizację
				var success = await _updateMealLogHandler.Handle(command);

				if (!success)
					return NotFound("Wpis posiłku nie został znaleziony lub nie masz uprawnień do jego edycji");

				return Ok(new { Message = "Wpis posiłku został pomyślnie zaktualizowany" });
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		/// <summary>
		/// Usuwa wpis posiłku z dziennika żywieniowego użytkownika.
		/// Operacja jest nieodwracalna - wpis zostanie trwale usunięty z systemu.
		/// </summary>
		/// <param name="id">Identyfikator wpisu posiłku do usunięcia.</param>
		/// <returns>
		/// 204 NoContent - wpis został pomyślnie usunięty.
		/// 401 Unauthorized - użytkownik nie jest zalogowany.
		/// 403 Forbidden - profil użytkownika nie jest kompletny.
		/// 404 NotFound - wpis posiłku nie istnieje lub nie należy do użytkownika.
		/// </returns>
		[HttpDelete("meals/{id:guid}")]
		public async Task<ActionResult> DeleteMealLog(Guid id)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId is null)
				return Unauthorized("Brak identyfikatora użytkownika w tokenie");

			// Tworzy komendę usuwania
			var command = new DeleteMealLogCommand(id, userId);

			// Wykonuje usuwanie
			var success = await _deleteMealLogHandler.Handle(command);

			if (!success)
				return NotFound("Wpis posiłku nie został znaleziony lub nie masz uprawnień do jego usunięcia");

			return NoContent();
		}

		/// <summary>
		/// Dodaje nowy wpis spożycia wody do dziennika użytkownika.
		/// Umożliwia śledzenie hydratacji i monitorowanie dziennego spożycia płynów.
		/// </summary>
		/// <param name="request">Dane nowego wpisu spożycia wody.</param>
		/// <returns>
		/// 201 Created z ID utworzonego wpisu - wpis został pomyślnie zalogowany.
		/// 400 BadRequest - nieprawidłowe dane wejściowe (nieprawidłowa ilość, etc.).
		/// 401 Unauthorized - użytkownik nie jest zalogowany.
		/// 403 Forbidden - profil użytkownika nie jest kompletny.
		/// </returns>
		[HttpPost("log-water")]
		public async Task<ActionResult<object>> LogWaterIntake([FromBody] LogWaterIntakeRequest request)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId is null)
				return Unauthorized("Brak identyfikatora użytkownika w tokenie");

			try
			{
				// Mapuje request na komendę
				var command = _mapper.Map<LogWaterIntakeCommand>(request);
				command.UserId = userId;

				// Wykonuje komendę przez handler
				var waterIntakeEntryId = await _logWaterIntakeHandler.Handle(command);

				// Zwraca sukces
				return Created($"/api/nutrition-tracking/water/{waterIntakeEntryId}", new
				{
					Id = waterIntakeEntryId,
					Message = "Wpis spożycia wody został pomyślnie zalogowany"
				});
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		/// <summary>
		/// Aktualizuje istniejący wpis spożycia wody użytkownika.
		/// Pozwala na modyfikację ilości, czasu spożycia i notatek.
		/// </summary>
		/// <param name="id">Identyfikator wpisu spożycia wody do aktualizacji.</param>
		/// <param name="request">Nowe dane wpisu spożycia wody.</param>
		/// <returns>
		/// 200 OK - wpis został pomyślnie zaktualizowany.
		/// 400 BadRequest - nieprawidłowe dane wejściowe.
		/// 401 Unauthorized - użytkownik nie jest zalogowany.
		/// 403 Forbidden - profil użytkownika nie jest kompletny.
		/// 404 NotFound - wpis spożycia wody nie istnieje lub nie należy do użytkownika.
		/// </returns>
		[HttpPut("water/{id:guid}")]
		public async Task<ActionResult> UpdateWaterIntake(Guid id, [FromBody] UpdateWaterIntakeRequest request)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId is null)
				return Unauthorized("Brak identyfikatora użytkownika w tokenie");

			try
			{
				// Mapuje request na komendę
				var command = _mapper.Map<UpdateWaterIntakeCommand>(request);
				command.WaterIntakeLogEntryId = id;
				command.UserId = userId;

				// Wykonuje aktualizację
				var success = await _updateWaterIntakeHandler.Handle(command);

				if (!success)
					return NotFound("Wpis spożycia wody nie został znaleziony lub nie masz uprawnień do jego edycji");

				return Ok(new { Message = "Wpis spożycia wody został pomyślnie zaktualizowany" });
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		/// <summary>
		/// Usuwa wpis spożycia wody z dziennika użytkownika.
		/// Operacja jest nieodwracalna - wpis zostanie trwale usunięty z systemu.
		/// </summary>
		/// <param name="id">Identyfikator wpisu spożycia wody do usunięcia.</param>
		/// <returns>
		/// 204 NoContent - wpis został pomyślnie usunięty.
		/// 401 Unauthorized - użytkownik nie jest zalogowany.
		/// 403 Forbidden - profil użytkownika nie jest kompletny.
		/// 404 NotFound - wpis spożycia wody nie istnieje lub nie należy do użytkownika.
		/// </returns>
		[HttpDelete("water/{id:guid}")]
		public async Task<ActionResult> DeleteWaterIntake(Guid id)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId is null)
				return Unauthorized("Brak identyfikatora użytkownika w tokenie");

			// Tworzy komendę usuwania
			var command = new DeleteWaterIntakeCommand(id, userId);

			// Wykonuje usuwanie
			var success = await _deleteWaterIntakeHandler.Handle(command);

			if (!success)
				return NotFound("Wpis spożycia wody nie został znaleziony lub nie masz uprawnień do jego usunięcia");

			return NoContent();
		}

		/// <summary>
		/// Pobiera predefiniowane opcje szybkiego dodawania wody.
		/// Zwraca listę popularnych ilości (szklanka, butelka, etc.) dla ułatwienia użytkownikowi.
		/// </summary>
		/// <returns>
		/// 200 OK z listą szybkich opcji dodawania wody.
		/// 401 Unauthorized - użytkownik nie jest zalogowany.
		/// </returns>
		[HttpGet("water/quick-options")]
		public ActionResult<List<QuickWaterIntakeDto>> GetQuickWaterOptions()
		{
			var quickOptions = new List<QuickWaterIntakeDto>
			{
				new() { Name = "Szklanka", AmountMilliliters = 250, Description = "Standardowa szklanka wody" },
				new() { Name = "Mała butelka", AmountMilliliters = 330, Description = "Mała butelka wody" },
				new() { Name = "Butelka", AmountMilliliters = 500, Description = "Standardowa butelka wody" },
				new() { Name = "Duża butelka", AmountMilliliters = 750, Description = "Duża butelka wody" },
				new() { Name = "Butelka 1L", AmountMilliliters = 1000, Description = "Litrowa butelka wody" },
				new() { Name = "Kubek", AmountMilliliters = 200, Description = "Mały kubek" },
				new() { Name = "Duży kubek", AmountMilliliters = 350, Description = "Duży kubek lub filiżanka" }
			};

			return Ok(quickOptions);
		}
	}
}