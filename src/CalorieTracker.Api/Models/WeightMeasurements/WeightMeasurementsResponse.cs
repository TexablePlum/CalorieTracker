namespace CalorieTracker.Api.Models.WeightMeasurements
{
	/// <summary>
	/// Response dla listy pomiarów z paginacją
	/// </summary>
	public class WeightMeasurementsResponse
	{
		public List<WeightMeasurementDto> Measurements { get; set; } = new();
		public int TotalCount { get; set; }
		public bool HasMore { get; set; }
	}
}
