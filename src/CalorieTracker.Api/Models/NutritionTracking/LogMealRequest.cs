// Plik LogMealRequest.cs - model żądania dodania posiłku do dziennika.
// DTO dla endpointu POST /api/nutrition-tracking/log-meal.

namespace CalorieTracker.Api.Models.NutritionTracking
{
	/// <summary>
	/// Model żądania dodania nowego posiłku do dziennika żywieniowego.
	/// Zawiera dane produktu/przepisu, ilości i czasu spożycia.
	/// </summary>
	public class LogMealRequest
	{
		/// <summary>
		/// Identyfikator produktu spożywczego (opcjonalny - XOR z RecipeId).
		/// </summary>
		public Guid? ProductId { get; set; }

		/// <summary>
		/// Identyfikator przepisu kulinarnego (opcjonalny - XOR z ProductId).
		/// </summary>
		public Guid? RecipeId { get; set; }

		/// <summary>
		/// Ilość spożytego produktu/przepisu w jednostkach bazowych.
		/// </summary>
		public float Quantity { get; set; }

		/// <summary>
		/// Typ posiłku jako string (Breakfast, Lunch, Dinner, etc.).
		/// </summary>
		public string MealType { get; set; } = null!;

		/// <summary>
		/// Data i czas spożycia posiłku (opcjonalny - domyślnie teraz).
		/// </summary>
		public DateTime? ConsumedAt { get; set; }

		/// <summary>
		/// Opcjonalne notatki użytkownika dotyczące posiłku.
		/// </summary>
		public string? Notes { get; set; }
	}
}