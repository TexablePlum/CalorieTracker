using CalorieTracker.Application.Interfaces;
using CalorieTracker.Application.WeightMeasurements.Queries;
using CalorieTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.WeightMeasurements.Handlers
{
	/// <summary>
	/// Handler do pobierania pomiarów masy ciała użytkownika
	/// </summary>
	public class GetUserWeightMeasurementsHandler
	{
		private readonly IAppDbContext _db;

		public GetUserWeightMeasurementsHandler(IAppDbContext db) => _db = db;

		public async Task<List<WeightMeasurement>> Handle(GetUserWeightMeasurementsQuery query)
		{
			return await _db.WeightMeasurements
				.Where(w => w.UserId == query.UserId)
				.OrderByDescending(w => w.MeasurementDate)
				.Skip(query.Skip)
				.Take(query.Take)
				.ToListAsync();
		}
	}
}