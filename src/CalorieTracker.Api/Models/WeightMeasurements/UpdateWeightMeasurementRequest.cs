namespace CalorieTracker.Api.Models.WeightMeasurements
{
	/// <summary>
	/// DTO do aktualizacji pomiaru masy ciała
	/// </summary>
	public class UpdateWeightMeasurementRequest
	{
		public DateOnly MeasurementDate { get; set; }
		public float WeightKg { get; set; }
	}
}
