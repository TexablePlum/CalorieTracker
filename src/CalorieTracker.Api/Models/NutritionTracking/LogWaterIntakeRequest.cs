// Plik LogWaterIntakeRequest.cs - model żądania dodania wpisu spożycia wody.

namespace CalorieTracker.Api.Models.NutritionTracking
{
	/// <summary>
	/// Model żądania dodania nowego wpisu spożycia wody do dziennika użytkownika.
	/// </summary>
	public class LogWaterIntakeRequest
	{
		/// <summary>
		/// Ilość wypitej wody w mililitrach.
		/// Typowe wartości: 250ml (szklanka), 500ml (butelka), 1000ml (duża butelka).
		/// </summary>
		public float AmountMilliliters { get; set; }

		/// <summary>
		/// Data i czas spożycia wody (opcjonalny - domyślnie teraz).
		/// </summary>
		public DateTime? ConsumedAt { get; set; }

		/// <summary>
		/// Opcjonalne notatki użytkownika dotyczące spożycia wody.
		/// </summary>
		public string? Notes { get; set; }
	}
}