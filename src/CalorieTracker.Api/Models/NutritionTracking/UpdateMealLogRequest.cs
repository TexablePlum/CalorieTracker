// Plik UpdateMealLogRequest.cs - model żądania aktualizacji wpisu posiłku.

namespace CalorieTracker.Api.Models.NutritionTracking
{
	/// <summary>
	/// Model żądania aktualizacji istniejącego wpisu posiłku.
	/// Zawiera nowe wartości ilości, typu posiłku, czasu i notatek.
	/// </summary>
	public class UpdateMealLogRequest
	{
		/// <summary>
		/// Nowa ilość spożytego produktu/przepisu.
		/// </summary>
		public float Quantity { get; set; }

		/// <summary>
		/// Nowy typ posiłku jako string.
		/// </summary>
		public string MealType { get; set; } = null!;

		/// <summary>
		/// Nowy czas spożycia posiłku.
		/// </summary>
		public DateTime ConsumedAt { get; set; }

		/// <summary>
		/// Nowe notatki użytkownika.
		/// </summary>
		public string? Notes { get; set; }
	}
}