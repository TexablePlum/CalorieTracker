// Plik WeightMeasurementRecalculationService.cs - prosty serwis do przeliczania pomiarów wagi.
// Centralizuje tylko duplikującą się logikę bez niepotrzebnego rozbicia.

using CalorieTracker.Application.Interfaces;
using CalorieTracker.Domain.Entities;
using CalorieTracker.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.WeightMeasurements.Services
{
	/// <summary>
	/// Serwis aplikacji do przeliczania pomiarów masy ciała.
	/// Centralizuje tylko duplikującą się logikę przeliczania.
	/// </summary>
	public class WeightMeasurementRecalculationService
	{
		private readonly IAppDbContext _db;
		private readonly WeightAnalysisService _weightAnalysis;

		public WeightMeasurementRecalculationService(IAppDbContext db, WeightAnalysisService weightAnalysis)
		{
			_db = db;
			_weightAnalysis = weightAnalysis;
		}

		/// <summary>
		/// Przelicza pomiary od podanej daty (używane po CREATE).
		/// </summary>
		public async Task RecalculateFromDate(string userId, DateOnly fromDate, UserProfile userProfile)
		{
			var allMeasurements = await GetAllUserMeasurementsOrdered(userId);
			var measurementsToRecalculate = allMeasurements.Where(m => m.MeasurementDate > fromDate).ToList();
			await RecalculateMeasurements(measurementsToRecalculate, allMeasurements, userProfile);
		}

		/// <summary>
		/// Przelicza pomiary od najwcześniejszej z dwóch dat (używane po UPDATE).
		/// </summary>
		public async Task RecalculateFromEarliest(string userId, DateOnly date1, DateOnly date2, UserProfile userProfile)
		{
			var earliestDate = date1 < date2 ? date1 : date2;
			var allMeasurements = await GetAllUserMeasurementsOrdered(userId);
			var measurementsToRecalculate = allMeasurements.Where(m => m.MeasurementDate >= earliestDate).ToList();
			await RecalculateMeasurements(measurementsToRecalculate, allMeasurements, userProfile);
		}

		/// <summary>
		/// Przelicza pomiary po podanej dacie (używane po DELETE).
		/// </summary>
		public async Task RecalculateAfterDate(string userId, DateOnly afterDate, UserProfile userProfile)
		{
			var allMeasurements = await GetAllUserMeasurementsOrdered(userId);
			var measurementsToRecalculate = allMeasurements.Where(m => m.MeasurementDate > afterDate).ToList();
			await RecalculateMeasurements(measurementsToRecalculate, allMeasurements, userProfile);
		}

		private async Task<List<WeightMeasurement>> GetAllUserMeasurementsOrdered(string userId)
		{
			return await _db.WeightMeasurements
				.Where(w => w.UserId == userId)
				.OrderBy(w => w.MeasurementDate)
				.ThenBy(w => w.CreatedAt)
				.ToListAsync();
		}

		private async Task RecalculateMeasurements(List<WeightMeasurement> measurementsToRecalculate,
			List<WeightMeasurement> allMeasurements, UserProfile userProfile)
		{
			foreach (var measurement in measurementsToRecalculate)
			{
				var previousMeasurement = allMeasurements
					.Where(m => m.MeasurementDate < measurement.MeasurementDate)
					.OrderByDescending(m => m.MeasurementDate)
					.ThenByDescending(m => m.CreatedAt)
					.FirstOrDefault();

				_weightAnalysis.FillCalculatedFields(measurement, userProfile, previousMeasurement);
				measurement.UpdatedAt = DateTime.UtcNow;
			}

			await _db.SaveChangesAsync();
		}
	}
}