using CalorieTracker.Application.Interfaces;
using CalorieTracker.Application.WeightMeasurements.Queries;
using CalorieTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.WeightMeasurements.Handlers
{
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
