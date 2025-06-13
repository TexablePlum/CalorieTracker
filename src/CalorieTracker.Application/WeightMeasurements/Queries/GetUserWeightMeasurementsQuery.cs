using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalorieTracker.Application.WeightMeasurements.Queries
{
	/// <summary>
	/// Query do pobrania pomiarów użytkownika z paginacją
	/// </summary>
	public class GetUserWeightMeasurementsQuery
	{
		public string UserId { get; init; } = null!;
		public int Skip { get; init; } = 0;
		public int Take { get; init; } = 20;
	}
}
