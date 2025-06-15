using CalorieTracker.Application.Interfaces;
using CalorieTracker.Application.WeightMeasurements.Commands;
using CalorieTracker.Domain.Entities;
using CalorieTracker.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.WeightMeasurements.Handlers
{
	/// <summary>
	/// Handler do usuwania pomiaru masy ciała
	/// </summary>
	public class DeleteWeightMeasurementHandler
	{
		private readonly IAppDbContext _db;
		private readonly WeightAnalysisService _weightAnalysis;

		public DeleteWeightMeasurementHandler(IAppDbContext db, WeightAnalysisService weightAnalysis)
		{
			_db = db;
			_weightAnalysis = weightAnalysis;
		}

		public async Task<bool> Handle(DeleteWeightMeasurementCommand command)
		{
			var measurement = await _db.WeightMeasurements
				.FirstOrDefaultAsync(w => w.Id == command.Id && w.UserId == command.UserId);

			if (measurement is null) return false;

			var deletedDate = measurement.MeasurementDate;
			var userId = command.UserId;

			// Pobierz profil użytkownika
			var userProfile = await _db.UserProfiles
				.FirstOrDefaultAsync(p => p.UserId == userId);

			if (userProfile is null) return false;

			// Usuń pomiar
			_db.WeightMeasurements.Remove(measurement);

			// ZAPISZ USUNIĘCIE NAJPIERW!
			await _db.SaveChangesAsync();

			// PRZELICZ WSZYSTKIE POMIARY PÓŹNIEJSZE NIŻ USUNIĘTY
			await RecalculateFutureMeasurements(userId, deletedDate, userProfile);

			// Jeśli usuwamy najnowszy pomiar, zaktualizuj profil użytkownika
			var latestMeasurement = await _db.WeightMeasurements
				.Where(w => w.UserId == userId)
				.OrderByDescending(w => w.MeasurementDate)
				.FirstOrDefaultAsync();

			if (latestMeasurement != null)
			{
				userProfile.WeightKg = latestMeasurement.WeightKg;
			}
			else
			{
				// Brak pomiarów - wyczyść wagę w profilu? Lub zostaw starą wartość
				// userProfile.WeightKg = null; // Opcjonalnie
			}

			await _db.SaveChangesAsync();
			return true;
		}

		/// <summary>
		/// Przelicza wszystkie pomiary późniejsze niż usunięty - POPRAWIONA WERSJA!
		/// </summary>
		private async Task RecalculateFutureMeasurements(string userId, DateOnly deletedDate, UserProfile userProfile)
		{
			// Pobierz WSZYSTKIE pomiary użytkownika w kolejności chronologicznej
			var allMeasurements = await _db.WeightMeasurements
				.Where(w => w.UserId == userId)
				.OrderBy(w => w.MeasurementDate)
				.ThenBy(w => w.CreatedAt)
				.ToListAsync();

			// Pomiary do przeliczenia (późniejsze niż usunięty)
			var measurementsToRecalculate = allMeasurements
				.Where(m => m.MeasurementDate > deletedDate)
				.ToList();

			// Przelicz każdy pomiar POPRAWNIE!
			foreach (var measurementToRecalc in measurementsToRecalculate)
			{
				// Znajdź poprzedni pomiar (najnowszy przed tym pomiarem)
				var previousMeasurement = allMeasurements
					.Where(m => m.MeasurementDate < measurementToRecalc.MeasurementDate)
					.OrderByDescending(m => m.MeasurementDate)
					.ThenByDescending(m => m.CreatedAt)
					.FirstOrDefault();

				// Przelicz kalkulowane pola
				_weightAnalysis.FillCalculatedFields(measurementToRecalc, userProfile, previousMeasurement);
				measurementToRecalc.UpdatedAt = DateTime.UtcNow;
			}

			// Zapisz wszystkie zmiany
			await _db.SaveChangesAsync();
		}
	}
}