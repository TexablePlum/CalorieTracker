namespace CalorieTracker.Api.Models.WeightMeasurements
{
	/// <summary>
	/// DTO do tworzenia nowego pomiaru masy ciała
	/// </summary>
	public class CreateWeightMeasurementRequest
	{
		public DateOnly MeasurementDate { get; set; }
		public float WeightKg { get; set; }
	}
}
