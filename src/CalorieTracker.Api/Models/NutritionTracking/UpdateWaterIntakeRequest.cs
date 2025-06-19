namespace CalorieTracker.Api.Models.NutritionTracking
{
	/// <summary>
	/// Model żądania aktualizacji wpisu spożycia wody.
	/// </summary>
	public class UpdateWaterIntakeRequest
	{
		/// <summary>
		/// Nowa ilość wypitej wody w mililitrach.
		/// </summary>
		public float AmountMilliliters { get; set; }

		/// <summary>
		/// Nowa data i czas spożycia wody.
		/// </summary>
		public DateTime ConsumedAt { get; set; }

		/// <summary>
		/// Nowe notatki użytkownika.
		/// </summary>
		public string? Notes { get; set; }
	}
}
