using CalorieTracker.Application.Interfaces;
using CalorieTracker.Application.WeightMeasurements.Queries;
using CalorieTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.WeightMeasurements.Handlers
{
	/// <summary>
	/// Handler do pobierania najnowszego pomiaru masy ciała użytkownika
	/// </summary>
	public class GetLatestWeightMeasurementHandler
	{
		private readonly IAppDbContext _db;

		public GetLatestWeightMeasurementHandler(IAppDbContext db) => _db = db;

		public async Task<WeightMeasurement?> Handle(GetLatestWeightMeasurementQuery query)
		{
			return await _db.WeightMeasurements
				.Where(w => w.UserId == query.UserId)
				.OrderByDescending(w => w.MeasurementDate)
				.FirstOrDefaultAsync();
		}
	}
}