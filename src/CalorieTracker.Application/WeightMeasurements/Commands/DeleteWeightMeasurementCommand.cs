using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalorieTracker.Application.WeightMeasurements.Commands
{
	/// <summary>
	/// Komenda do usunięcia pomiaru masy ciała
	/// </summary>
	public record DeleteWeightMeasurementCommand(Guid Id, string UserId);
}
