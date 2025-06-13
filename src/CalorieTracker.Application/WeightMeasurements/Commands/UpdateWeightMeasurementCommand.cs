using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalorieTracker.Application.WeightMeasurements.Commands
{
	/// <summary>
	/// Komenda do aktualizacji pomiaru masy ciała
	/// </summary>
	public record UpdateWeightMeasurementCommand
	{
		public Guid Id { get; init; }
		public string UserId { get; init; } = null!;
		public DateOnly MeasurementDate { get; init; }
		public float WeightKg { get; init; }
	}
}
