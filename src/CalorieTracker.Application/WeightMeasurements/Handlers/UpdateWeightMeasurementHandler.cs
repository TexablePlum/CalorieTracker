using CalorieTracker.Application.Interfaces;
using CalorieTracker.Application.WeightMeasurements.Commands;
using CalorieTracker.Application.WeightMeasurements.Queries;
using CalorieTracker.Domain.Entities;
using CalorieTracker.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.WeightMeasurements.Handlers
{
	/// <summary>
	/// Handler do aktualizacji pomiaru masy ciała
	/// </summary>
	public class UpdateWeightMeasurementHandler
	{
		private readonly IAppDbContext _db;
		private readonly WeightAnalysisService _weightAnalysis;

		public UpdateWeightMeasurementHandler(IAppDbContext db, WeightAnalysisService weightAnalysis)
		{
			_db = db;
			_weightAnalysis = weightAnalysis;
		}

		public async Task<bool> Handle(UpdateWeightMeasurementCommand command)
		{
			var measurement = await _db.WeightMeasurements
				.FirstOrDefaultAsync(w => w.Id == command.Id && w.UserId == command.UserId);

			if (measurement is null) return false;

			// Pobierz profil użytkownika
			var userProfile = await _db.UserProfiles
				.FirstOrDefaultAsync(p => p.UserId == command.UserId);

			if (userProfile is null) return false;

			// Pobierz poprzedni pomiar
			var previousMeasurement = await _db.WeightMeasurements
				.Where(w => w.UserId == command.UserId && w.MeasurementDate < command.MeasurementDate && w.Id != command.Id)
				.OrderByDescending(w => w.MeasurementDate)
				.FirstOrDefaultAsync();

			// Aktualizuj dane
			measurement.MeasurementDate = command.MeasurementDate;
			measurement.WeightKg = command.WeightKg;
			measurement.UpdatedAt = DateTime.UtcNow;

			// Przelicz kalkulowane pola
			_weightAnalysis.FillCalculatedFields(measurement, userProfile, previousMeasurement);

			// Sprawdź czy to najnowszy pomiar - jeśli tak, zaktualizuj profil
			var latestMeasurement = await _db.WeightMeasurements
				.Where(w => w.UserId == command.UserId)
				.OrderByDescending(w => w.MeasurementDate)
				.FirstOrDefaultAsync();

			if (latestMeasurement?.Id == measurement.Id)
			{
				userProfile.WeightKg = command.WeightKg;
			}

			await _db.SaveChangesAsync();
			return true;
		}
	}

	/// <summary>
	/// Handler do usuwania pomiaru masy ciała
	/// </summary>
	public class DeleteWeightMeasurementHandler
	{
		private readonly IAppDbContext _db;

		public DeleteWeightMeasurementHandler(IAppDbContext db) => _db = db;

		public async Task<bool> Handle(DeleteWeightMeasurementCommand command)
		{
			var measurement = await _db.WeightMeasurements
				.FirstOrDefaultAsync(w => w.Id == command.Id && w.UserId == command.UserId);

			if (measurement is null) return false;

			_db.WeightMeasurements.Remove(measurement);

			// Jeśli usuwamy najnowszy pomiar, zaktualizuj profil użytkownika
			var latestMeasurement = await _db.WeightMeasurements
				.Where(w => w.UserId == command.UserId && w.Id != command.Id)
				.OrderByDescending(w => w.MeasurementDate)
				.FirstOrDefaultAsync();

			var userProfile = await _db.UserProfiles
				.FirstOrDefaultAsync(p => p.UserId == command.UserId);

			if (userProfile != null)
			{
				userProfile.WeightKg = latestMeasurement?.WeightKg ?? userProfile.WeightKg;
			}

			await _db.SaveChangesAsync();
			return true;
		}
	}

	/// <summary>
	/// Handler do pobierania szczegółów pomiaru
	/// </summary>
	public class GetWeightMeasurementDetailsHandler
	{
		private readonly IAppDbContext _db;

		public GetWeightMeasurementDetailsHandler(IAppDbContext db) => _db = db;

		public async Task<WeightMeasurement?> Handle(GetWeightMeasurementDetailsQuery query)
		{
			return await _db.WeightMeasurements
				.FirstOrDefaultAsync(w => w.Id == query.Id && w.UserId == query.UserId);
		}
	}
}