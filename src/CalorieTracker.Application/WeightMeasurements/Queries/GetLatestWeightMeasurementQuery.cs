using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalorieTracker.Application.WeightMeasurements.Queries
{
	/// <summary>
	/// Query do pobrania najnowszego pomiaru użytkownika
	/// </summary>
	public record GetLatestWeightMeasurementQuery(string UserId);
}
