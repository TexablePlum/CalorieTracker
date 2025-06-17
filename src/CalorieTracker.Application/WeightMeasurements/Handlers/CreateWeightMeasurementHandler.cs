// Plik CreateWeightMeasurementHandler.cs - implementacja handlera tworzenia pomiaru wagi.
// Odpowiada za przetwarzanie komendy CreateWeightMeasurementCommand i zarządzanie logiką biznesową związaną z tworzeniem nowych pomiarów wagi.

using CalorieTracker.Application.Interfaces;
using CalorieTracker.Application.WeightMeasurements.Commands;
using CalorieTracker.Application.WeightMeasurements.Services;
using CalorieTracker.Domain.Entities;
using CalorieTracker.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.WeightMeasurements.Handlers
{
	/// <summary>
	/// Handler odpowiedzialny za przetwarzanie komendy tworzenia nowego pomiaru masy ciała.
	/// Zarządza całą logiką związaną z walidacją, tworzeniem i aktualizacją pomiarów wagi.
	/// Automatycznie przelicza kalkulowane pola (BMI, zmiany wagi) oraz aktualizuje kolejne pomiary w sekwencji.
	/// </summary>
	public class CreateWeightMeasurementHandler
	{
		/// <summary>
		/// Kontekst bazy danych do zarządzania pomiarami wagi i profilami użytkowników.
		/// </summary>
		private readonly IAppDbContext _db;

		/// <summary>
		/// Serwis analizy masy ciała do kalkulacji BMI i zmian wagi.
		/// </summary>
		private readonly WeightAnalysisService _weightAnalysis;

		/// <summary>
		/// Serwis przeliczania pomiarów odpowiedzialny za aktualizację kalkulowanych pól w sekwencji pomiarów.
		/// </summary>
		private readonly WeightMeasurementRecalculationService _recalculationService;

		/// <summary>
		/// Inicjalizuje nową instancję handlera tworzenia pomiaru wagi.
		/// </summary>
		/// <param name="db">Kontekst bazy danych <see cref="IAppDbContext"/>.</param>
		/// <param name="weightAnalysis">Serwis analizy masy ciała <see cref="WeightAnalysisService"/>.</param>
		/// <param name="recalculationService">Serwis przeliczania pomiarów <see cref="WeightMeasurementRecalculationService"/>.</param>
		public CreateWeightMeasurementHandler(
			IAppDbContext db,
			WeightAnalysisService weightAnalysis,
			WeightMeasurementRecalculationService recalculationService)
		{
			_db = db;
			_weightAnalysis = weightAnalysis;
			_recalculationService = recalculationService;
		}

		/// <summary>
		/// Przetwarza komendę tworzenia nowego pomiaru masy ciała.
		/// Wykonuje walidację wymagań biznesowych, tworzy pomiar z automatycznym obliczaniem BMI i zmian wagi,
		/// przelicza wszystkie późniejsze pomiary oraz aktualizuje profil użytkownika jeśli jest to najnowszy pomiar.
		/// </summary>
		/// <param name="command">Komenda <see cref="CreateWeightMeasurementCommand"/> zawierająca dane nowego pomiaru masy ciała.</param>
		/// <returns>
		/// Task asynchroniczny zwracający unikalny identyfikator GUID nowo utworzonego pomiaru masy ciała.
		/// </returns>
		/// <exception cref="InvalidOperationException">
		/// Wyjątek wyrzucany w następujących przypadkach:
		/// - Użytkownik nie ma uzupełnionego profilu (wymagany do kalkulacji BMI)
		/// - Pomiar na podaną datę już istnieje w systemie
		/// </exception>
		public async Task<Guid> Handle(CreateWeightMeasurementCommand command)
		{
			// Sprawdza czy użytkownik ma profil
			var userProfile = await _db.UserProfiles
				.FirstOrDefaultAsync(p => p.UserId == command.UserId);

			if (userProfile is null)
				throw new InvalidOperationException("Użytkownik musi mieć uzupełniony profil aby dodawać pomiary masy ciała");

			// Sprawdza czy nie ma już pomiaru na tę datę
			var existingMeasurement = await _db.WeightMeasurements
				.FirstOrDefaultAsync(w => w.UserId == command.UserId && w.MeasurementDate == command.MeasurementDate);

			if (existingMeasurement != null)
				throw new InvalidOperationException("Pomiar na tę datę już istnieje");

			// Pobiera poprzedni pomiar
			var previousMeasurement = await _db.WeightMeasurements
				.Where(w => w.UserId == command.UserId && w.MeasurementDate < command.MeasurementDate)
				.OrderByDescending(w => w.MeasurementDate)
				.FirstOrDefaultAsync();

			// Tworzy nowy pomiar
			var measurement = new WeightMeasurement
			{
				UserId = command.UserId,
				MeasurementDate = command.MeasurementDate,
				WeightKg = command.WeightKg,
				CreatedAt = DateTime.UtcNow,
				UpdatedAt = DateTime.UtcNow
			};

			_weightAnalysis.FillCalculatedFields(measurement, userProfile, previousMeasurement);
			_db.WeightMeasurements.Add(measurement);
			await _db.SaveChangesAsync();

			// Przelicza przyszłe pomiary
			await _recalculationService.RecalculateFromDate(command.UserId, command.MeasurementDate, userProfile);

			// Aktualizuje profil jeśli to najnowszy pomiar
			var latestMeasurement = await _db.WeightMeasurements
				.Where(w => w.UserId == command.UserId)
				.OrderByDescending(w => w.MeasurementDate)
				.FirstOrDefaultAsync();

			if (latestMeasurement?.Id == measurement.Id)
			{
				userProfile.WeightKg = command.WeightKg;
				await _db.SaveChangesAsync();
			}

			return measurement.Id;
		}
	}
}