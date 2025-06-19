// Plik MealLogEntryDto.cs - szczegółowy DTO wpisu posiłku.

namespace CalorieTracker.Api.Models.NutritionTracking
{
	/// <summary>
	/// Model transferu danych reprezentujący szczegółowy wpis posiłku.
	/// Używany w historii posiłków i operacjach CRUD.
	/// </summary>
	public class MealLogEntryDto
	{
		/// <summary>
		/// Identyfikator wpisu posiłku.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Data wpisu w formacie yyyy-MM-dd.
		/// </summary>
		public string Date { get; set; } = null!;

		/// <summary>
		/// Typ posiłku jako string.
		/// </summary>
		public string MealType { get; set; } = null!;

		/// <summary>
		/// Nazwa produktu lub przepisu.
		/// </summary>
		public string Name { get; set; } = null!;

		/// <summary>
		/// Identyfikator produktu (jeśli dotyczy).
		/// </summary>
		public Guid? ProductId { get; set; }

		/// <summary>
		/// Identyfikator przepisu (jeśli dotyczy).
		/// </summary>
		public Guid? RecipeId { get; set; }

		/// <summary>
		/// Spożyta ilość.
		/// </summary>
		public float Quantity { get; set; }

		/// <summary>
		/// Jednostka miary.
		/// </summary>
		public string Unit { get; set; } = null!;

		/// <summary>
		/// Kalkulowane wartości odżywcze.
		/// </summary>
		public MealNutritionDto Nutrition { get; set; } = null!;

		/// <summary>
		/// Data i czas spożycia.
		/// </summary>
		public DateTime ConsumedAt { get; set; }

		/// <summary>
		/// Opcjonalne notatki.
		/// </summary>
		public string? Notes { get; set; }
	}
}