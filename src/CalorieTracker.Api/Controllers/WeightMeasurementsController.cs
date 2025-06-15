// Plik WeightMeasurementsController.cs - kontroler zarządzania pomiarami masy ciała w aplikacji CalorieTracker.
// Odpowiada za operacje CRUD pomiarów wagi oraz automatyczne kalkulacje postępu i analizy trendu masy ciała.

using AutoMapper;
using CalorieTracker.Api.Attributes;
using CalorieTracker.Api.Models.WeightMeasurements;
using CalorieTracker.Application.WeightMeasurements.Commands;
using CalorieTracker.Application.WeightMeasurements.Handlers;
using CalorieTracker.Application.WeightMeasurements.Queries;
using CalorieTracker.Domain.Services;
using CalorieTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CalorieTracker.Api.Controllers
{
	/// <summary>
	/// Kontroler pomiarów masy ciała zarządzający śledzeniem wagi i analizą postępu użytkowników.
	/// Obsługuje pełny cykl CRUD pomiarów z automatyczną kalkulacją BMI, postępu względem celu i analizą trendów.
	/// Wymaga autoryzacji oraz kompletnego profilu użytkownika dla wszystkich operacji.
	/// Implementuje wzorzec CQRS z dedykowanymi handlerami i integruje serwis analizy masy ciała.
	/// </summary>
	[Authorize]
	[RequireCompleteProfile]
	[ApiController]
	[Route("api/[controller]")]
	public class WeightMeasurementsController : ControllerBase
	{
		/// <summary>
		/// Mapper AutoMapper do konwersji między modelami API a komendami/zapytaniami domenowymi.
		/// </summary>
		private readonly IMapper _mapper;

		/// <summary>
		/// Serwis analizy masy ciała do kalkulacji BMI, postępu i statystyk wagowych.
		/// </summary>
		private readonly WeightAnalysisService _weightAnalysis;

		/// <summary>
		/// Kontekst bazy danych do bezpośredniego dostępu do profili użytkowników.
		/// </summary>
		private readonly IAppDbContext _db;

		/// <summary>
		/// Handler odpowiedzialny za tworzenie nowych pomiarów masy ciała.
		/// </summary>
		private readonly CreateWeightMeasurementHandler _createHandler;

		/// <summary>
		/// Handler zarządzający aktualizacją istniejących pomiarów wagi.
		/// </summary>
		private readonly UpdateWeightMeasurementHandler _updateHandler;

		/// <summary>
		/// Handler obsługujący usuwanie pomiarów masy ciała z systemu.
		/// </summary>
		private readonly DeleteWeightMeasurementHandler _deleteHandler;

		/// <summary>
		/// Handler pobierania pomiarów masy ciała konkretnego użytkownika z paginacją.
		/// </summary>
		private readonly GetUserWeightMeasurementsHandler _getUserMeasurementsHandler;

		/// <summary>
		/// Handler pobierania najnowszego pomiaru masy ciała użytkownika.
		/// </summary>
		private readonly GetLatestWeightMeasurementHandler _getLatestHandler;

		/// <summary>
		/// Handler pobierania szczegółowych informacji o konkretnym pomiarze.
		/// </summary>
		private readonly GetWeightMeasurementDetailsHandler _getDetailsHandler;

		/// <summary>
		/// Inicjalizuje nową instancję kontrolera pomiarów masy ciała z wymaganymi zależnościami.
		/// </summary>
		/// <param name="mapper">Mapper AutoMapper do konwersji obiektów.</param>
		/// <param name="weightAnalysis">Serwis analizy masy ciała i kalkulacji statystyk.</param>
		/// <param name="db">Kontekst bazy danych aplikacji.</param>
		/// <param name="createHandler">Handler tworzenia pomiarów.</param>
		/// <param name="updateHandler">Handler aktualizacji pomiarów.</param>
		/// <param name="deleteHandler">Handler usuwania pomiarów.</param>
		/// <param name="getUserMeasurementsHandler">Handler pobierania pomiarów użytkownika.</param>
		/// <param name="getLatestHandler">Handler pobierania najnowszego pomiaru.</param>
		/// <param name="getDetailsHandler">Handler pobierania szczegółów pomiaru.</param>
		public WeightMeasurementsController(
			   IMapper mapper,
			   WeightAnalysisService weightAnalysis,
			   IAppDbContext db,
			   CreateWeightMeasurementHandler createHandler,
			   UpdateWeightMeasurementHandler updateHandler,
			   DeleteWeightMeasurementHandler deleteHandler,
			   GetUserWeightMeasurementsHandler getUserMeasurementsHandler,
			   GetLatestWeightMeasurementHandler getLatestHandler,
			   GetWeightMeasurementDetailsHandler getDetailsHandler)
		{
			_mapper = mapper;
			_weightAnalysis = weightAnalysis;
			_db = db;
			_createHandler = createHandler;
			_updateHandler = updateHandler;
			_deleteHandler = deleteHandler;
			_getUserMeasurementsHandler = getUserMeasurementsHandler;
			_getLatestHandler = getLatestHandler;
			_getDetailsHandler = getDetailsHandler;
		}

		/// <summary>
		/// Pobiera listę pomiarów masy ciała aktualnie zalogowanego użytkownika.
		/// Automatycznie kalkuluje postęp względem celu wagowego dla każdego pomiaru.
		/// Implementuje paginację dla efektywnego przeglądania historii pomiarów.
		/// </summary>
		/// <param name="skip">Liczba pomiarów do pominięcia (domyślnie 0).</param>
		/// <param name="take">Liczba pomiarów do pobrania (domyślnie 20, maksymalnie 100).</param>
		/// <returns>
		/// 200 OK z listą pomiarów - pomiary z kalkulowanym postępem względem celu.
		/// 401 Unauthorized - gdy użytkownik nie jest zalogowany.
		/// 403 Forbidden - gdy profil użytkownika nie jest kompletny.
		/// </returns>
		[HttpGet]
		public async Task<ActionResult<WeightMeasurementsResponse>> GetWeightMeasurements([FromQuery] int skip = 0, [FromQuery] int take = 20)
		{
			// Ekstraktuje ID użytkownika z tokenu JWT
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId is null) return Unauthorized();

			// Pobiera pomiary użytkownika z paginacją
			var query = new GetUserWeightMeasurementsQuery { UserId = userId, Skip = skip, Take = take };
			var measurements = await _getUserMeasurementsHandler.Handle(query);

			// Pobiera profil użytkownika dla kalkulacji postępu względem celu
			var userProfile = await _db.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);

			// Przetwarza pomiary z automatyczną kalkulacją postępu
			var measurementDtos = new List<WeightMeasurementDto>();
			foreach (var measurement in measurements)
			{
				var dto = _mapper.Map<WeightMeasurementDto>(measurement);

				// Kalkuluje postęp względem celu wagowego użytkownika
				if (userProfile?.TargetWeightKg != null)
				{
					dto.ProgressToGoal = _weightAnalysis.CalculateProgressToGoal(measurement.WeightKg, userProfile.TargetWeightKg.Value);
				}

				measurementDtos.Add(dto);
			}

			return Ok(new WeightMeasurementsResponse
			{
				Measurements = measurementDtos,
				TotalCount = measurementDtos.Count,
				HasMore = measurementDtos.Count == take
			});
		}

		/// <summary>
		/// Pobiera najnowszy pomiar masy ciała użytkownika.
		/// Automatycznie kalkuluje aktualny postęp względem ustalonego celu wagowego.
		/// Przydatne do wyświetlania aktualnego stanu użytkownika na dashboardzie.
		/// </summary>
		/// <returns>
		/// 200 OK z najnowszym pomiarem - najnowszy pomiar z postępem względem celu.
		/// 404 NotFound - gdy użytkownik nie ma jeszcze żadnych pomiarów.
		/// 401 Unauthorized - gdy użytkownik nie jest zalogowany.
		/// 403 Forbidden - gdy profil użytkownika nie jest kompletny.
		/// </returns>
		[HttpGet("latest")]
		public async Task<ActionResult<WeightMeasurementDto>> GetLatestWeightMeasurement()
		{
			// Weryfikuje autoryzację użytkownika
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId is null) return Unauthorized();

			// Pobiera najnowszy pomiar użytkownika
			var measurement = await _getLatestHandler.Handle(new GetLatestWeightMeasurementQuery(userId));
			if (measurement is null) return NotFound("Brak pomiarów masy ciała");

			// Pobiera profil dla kalkulacji postępu
			var userProfile = await _db.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
			var dto = _mapper.Map<WeightMeasurementDto>(measurement);

			// Kalkuluje aktualny postęp względem celu wagowego
			if (userProfile?.TargetWeightKg != null)
			{
				dto.ProgressToGoal = _weightAnalysis.CalculateProgressToGoal(measurement.WeightKg, userProfile.TargetWeightKg.Value);
			}

			return Ok(dto);
		}

		/// <summary>
		/// Pobiera szczegółowe informacje o konkretnym pomiarze masy ciała.
		/// Weryfikuje własność pomiaru i automatycznie kalkuluje postęp względem celu.
		/// Zapewnia bezpieczeństwo dostępu - użytkownik może przeglądać tylko swoje pomiary.
		/// </summary>
		/// <param name="id">Unikalny identyfikator pomiaru (GUID).</param>
		/// <returns>
		/// 200 OK ze szczegółami pomiaru - pełne informacje o pomiarze z postępem.
		/// 404 NotFound - gdy pomiar nie istnieje lub nie należy do użytkownika.
		/// 401 Unauthorized - gdy użytkownik nie jest zalogowany.
		/// 403 Forbidden - gdy profil użytkownika nie jest kompletny.
		/// </returns>
		[HttpGet("{id:guid}")]
		public async Task<ActionResult<WeightMeasurementDto>> GetWeightMeasurement(Guid id)
		{
			// Sprawdza autoryzację użytkownika
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId is null) return Unauthorized();

			// Pobiera pomiar z weryfikacją własności
			var measurement = await _getDetailsHandler.Handle(new GetWeightMeasurementDetailsQuery(id, userId));
			if (measurement is null) return NotFound("Pomiar nie został znaleziony");

			// Pobiera profil użytkownika dla kalkulacji
			var userProfile = await _db.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
			var dto = _mapper.Map<WeightMeasurementDto>(measurement);

			// Kalkuluje postęp względem ustalonego celu
			if (userProfile?.TargetWeightKg != null)
			{
				dto.ProgressToGoal = _weightAnalysis.CalculateProgressToGoal(measurement.WeightKg, userProfile.TargetWeightKg.Value);
			}

			return Ok(dto);
		}

		/// <summary>
		/// Tworzy nowy pomiar masy ciała dla aktualnie zalogowanego użytkownika.
		/// Automatycznie kalkuluje BMI, zmiany względem poprzedniego pomiaru i postęp względem celu.
		/// Implementuje walidację biznesową zapobiegającą duplikacji pomiarów w tym samym dniu.
		/// </summary>
		/// <param name="request">Dane nowego pomiaru zawierające wagę, datę i opcjonalne notatki.</param>
		/// <returns>
		/// 201 Created z ID nowego pomiaru - gdy pomiar został pomyślnie utworzony.
		/// 400 BadRequest - gdy dane są nieprawidłowe lub istnieje już pomiar z danej daty.
		/// 401 Unauthorized - gdy użytkownik nie jest zalogowany.
		/// 403 Forbidden - gdy profil użytkownika nie jest kompletny.
		/// </returns>
		[HttpPost]
		public async Task<ActionResult<Guid>> CreateWeightMeasurement([FromBody] CreateWeightMeasurementRequest request)
		{
			// Weryfikuje autoryzację użytkownika
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId is null) return Unauthorized();

			// Przygotowuje komendę tworzenia pomiaru
			var command = _mapper.Map<CreateWeightMeasurementCommand>(request);
			command = command with { UserId = userId };

			try
			{
				// Wykonuje operację tworzenia z automatyczną kalkulacją
				var measurementId = await _createHandler.Handle(command);

				// Zwraca odpowiedź 201 Created z lokalizacją nowego zasobu
				return CreatedAtAction(
					   nameof(GetWeightMeasurement),
					   new { id = measurementId },
					   new { id = measurementId });
			}
			catch (InvalidOperationException ex)
			{
				// Obsługuje błędy biznesowe (np. duplikacja daty)
				return BadRequest(ex.Message);
			}
		}

		/// <summary>
		/// Aktualizuje istniejący pomiar masy ciała.
		/// Automatycznie przelicza wszystkie kalkulowane pola (BMI, zmiany, postęp).
		/// Weryfikuje własność pomiaru - użytkownik może edytować tylko swoje pomiary.
		/// </summary>
		/// <param name="id">Unikalny identyfikator pomiaru do aktualizacji.</param>
		/// <param name="request">Nowe dane pomiaru zastępujące istniejące wartości.</param>
		/// <returns>
		/// 204 NoContent - gdy aktualizacja przebiegła pomyślnie.
		/// 400 BadRequest - gdy dane są nieprawidłowe.
		/// 401 Unauthorized - gdy użytkownik nie jest zalogowany.
		/// 403 Forbidden - gdy profil użytkownika nie jest kompletny.
		/// 404 NotFound - gdy pomiar nie istnieje lub nie należy do użytkownika.
		/// </returns>
		[HttpPut("{id:guid}")]
		public async Task<IActionResult> UpdateWeightMeasurement(Guid id, [FromBody] UpdateWeightMeasurementRequest request)
		{
			// Sprawdza autoryzację użytkownika
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId is null) return Unauthorized();

			// Przygotowuje komendę aktualizacji z weryfikacją własności
			var command = _mapper.Map<UpdateWeightMeasurementCommand>(request);
			command = command with { Id = id, UserId = userId };

			// Wykonuje operację aktualizacji
			var success = await _updateHandler.Handle(command);

			// Sprawdza czy operacja zakończyła się sukcesem
			if (!success)
				return NotFound("Pomiar nie został znaleziony");

			return NoContent();
		}

		/// <summary>
		/// Usuwa pomiar masy ciała z systemu.
		/// Weryfikuje własność pomiaru - użytkownik może usuwać tylko swoje pomiary.
		/// Operacja jest nieodwracalna - usuwa pomiar permanentnie z bazy danych.
		/// </summary>
		/// <param name="id">Unikalny identyfikator pomiaru do usunięcia.</param>
		/// <returns>
		/// 204 NoContent - gdy usunięcie przebiegło pomyślnie.
		/// 401 Unauthorized - gdy użytkownik nie jest zalogowany.
		/// 403 Forbidden - gdy profil użytkownika nie jest kompletny.
		/// 404 NotFound - gdy pomiar nie istnieje lub nie należy do użytkownika.
		/// </returns>
		[HttpDelete("{id:guid}")]
		public async Task<IActionResult> DeleteWeightMeasurement(Guid id)
		{
			// Weryfikuje tożsamość użytkownika
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId is null) return Unauthorized();

			// Wykonuje operację usunięcia z weryfikacją własności
			var success = await _deleteHandler.Handle(new DeleteWeightMeasurementCommand(id, userId));

			// Sprawdza rezultat operacji
			if (!success)
				return NotFound("Pomiar nie został znaleziony");

			return NoContent();
		}
	}
}