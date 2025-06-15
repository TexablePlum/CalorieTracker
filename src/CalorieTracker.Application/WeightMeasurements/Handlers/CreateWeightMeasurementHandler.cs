using CalorieTracker.Application.Interfaces;
using CalorieTracker.Application.WeightMeasurements.Commands;
using CalorieTracker.Domain.Entities;
using CalorieTracker.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.WeightMeasurements.Handlers
{
	/// <summary>
	/// Handler do tworzenia nowego pomiaru masy ciała
	/// </summary>
	public class CreateWeightMeasurementHandler
	{
		private readonly IAppDbContext _db;
		private readonly WeightAnalysisService _weightAnalysis;

		public CreateWeightMeasurementHandler(IAppDbContext db, WeightAnalysisService weightAnalysis)
		{
			_db = db;
			_weightAnalysis = weightAnalysis;
		}

		public async Task<Guid> Handle(CreateWeightMeasurementCommand command)
		{
			// Sprawdź czy użytkownik ma profil
			var userProfile = await _db.UserProfiles
				.FirstOrDefaultAsync(p => p.UserId == command.UserId);

			if (userProfile is null)
			{
				throw new InvalidOperationException("Użytkownik musi mieć uzupełniony profil aby dodawać pomiary masy ciała");
			}

			// Sprawdź czy nie ma już pomiaru na tę datę
			var existingMeasurement = await _db.WeightMeasurements
				.FirstOrDefaultAsync(w => w.UserId == command.UserId && w.MeasurementDate == command.MeasurementDate);

			if (existingMeasurement != null)
			{
				throw new InvalidOperationException("Pomiar na tę datę już istnieje");
			}

			// Pobierz poprzedni pomiar (najnowszy przed tą datą)
			var previousMeasurement = await _db.WeightMeasurements
				.Where(w => w.UserId == command.UserId && w.MeasurementDate < command.MeasurementDate)
				.OrderByDescending(w => w.MeasurementDate)
				.FirstOrDefaultAsync();

			// Utwórz nowy pomiar
			var measurement = new WeightMeasurement
			{
				UserId = command.UserId,
				MeasurementDate = command.MeasurementDate,
				WeightKg = command.WeightKg,
				CreatedAt = DateTime.UtcNow,
				UpdatedAt = DateTime.UtcNow
			};

			// Wypełnij kalkulowane pola (BMI, zmiana wagi)
			_weightAnalysis.FillCalculatedFields(measurement, userProfile, previousMeasurement);

			// Dodaj do bazy
			_db.WeightMeasurements.Add(measurement);

			// KLUCZOWE: Przelicz wszystkie pomiary PÓŹNIEJSZE niż ten nowy
			await RecalculateFutureMeasurements(command.UserId, command.MeasurementDate, userProfile);

			// Aktualizuj wagę w profilu użytkownika jeśli to najnowszy pomiar
			var latestMeasurement = await _db.WeightMeasurements
				.Where(w => w.UserId == command.UserId)
				.OrderByDescending(w => w.MeasurementDate)
				.FirstOrDefaultAsync();

			if (latestMeasurement == null || command.MeasurementDate >= latestMeasurement.MeasurementDate)
			{
				userProfile.WeightKg = command.WeightKg;
			}

			await _db.SaveChangesAsync();

			return measurement.Id;
		}

		/// <summary>
		/// Przelicza wszystkie pomiary późniejsze niż podana data
		/// </summary>
		private async Task RecalculateFutureMeasurements(string userId, DateOnly fromDate, UserProfile userProfile)
		{
			// Pobierz WSZYSTKIE pomiary użytkownika w kolejności chronologicznej
			var allMeasurements = await _db.WeightMeasurements
				.Where(w => w.UserId == userId)
				.OrderBy(w => w.MeasurementDate)
				.ThenBy(w => w.CreatedAt)
				.ToListAsync();

			// Znajdź pomiary do przeliczenia (późniejsze niż fromDate)
			var measurementsToRecalculate = allMeasurements
				.Where(m => m.MeasurementDate > fromDate)
				.ToList();

			// Przelicz każdy pomiar
			foreach (var measurement in measurementsToRecalculate)
			{
				// Znajdź poprzedni pomiar (najnowszy przed tym pomiarem)
				var previousMeasurement = allMeasurements
					.Where(m => m.MeasurementDate < measurement.MeasurementDate)
					.OrderByDescending(m => m.MeasurementDate)
					.ThenByDescending(m => m.CreatedAt)
					.FirstOrDefault();

				// Przelicz kalkulowane pola
				_weightAnalysis.FillCalculatedFields(measurement, userProfile, previousMeasurement);
				measurement.UpdatedAt = DateTime.UtcNow;
			}
		}
	}
}