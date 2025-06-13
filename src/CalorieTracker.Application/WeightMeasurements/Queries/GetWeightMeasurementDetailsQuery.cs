using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalorieTracker.Application.WeightMeasurements.Queries
{
	/// <summary>
	/// Query do pobrania szczegółów konkretnego pomiaru
	/// </summary>
	public record GetWeightMeasurementDetailsQuery(Guid Id, string UserId);
}
