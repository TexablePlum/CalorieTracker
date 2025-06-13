namespace CalorieTracker.Api.Models.WeightMeasurements
{
	/// <summary>
	/// DTO zwracane przez API ze szczegółami pomiaru masy ciała
	/// </summary>
	public class WeightMeasurementDto
	{
		public Guid Id { get; set; }
		public DateOnly MeasurementDate { get; set; }
		public float WeightKg { get; set; }
		public float BMI { get; set; }
		public float WeightChangeKg { get; set; }
		public float ProgressToGoal { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}
