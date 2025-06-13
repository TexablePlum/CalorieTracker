using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalorieTracker.Application.WeightMeasurements.Commands
{
	/// <summary>
	/// Komenda do utworzenia nowego pomiaru masy ciała
	/// </summary>
	public record CreateWeightMeasurementCommand
	{
		public string UserId { get; init; } = null!;
		public DateOnly MeasurementDate { get; init; }
		public float WeightKg { get; init; }
	}
}
