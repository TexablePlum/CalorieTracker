// Plik MealLogSummaryDto.cs - DTO podsumowania posiłku.

namespace CalorieTracker.Api.Models.NutritionTracking
{
	/// <summary>
	/// Model transferu danych reprezentujący uproszczone informacje o posiłku.
	/// </summary>
	public class MealLogSummaryDto
	{
		/// <summary>
		/// Identyfikator wpisu posiłku.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Typ posiłku jako string.
		/// </summary>
		public string MealType { get; set; } = null!;

		/// <summary>
		/// Nazwa produktu lub przepisu.
		/// </summary>
		public string Name { get; set; } = null!;

		/// <summary>
		/// Spożyta ilość z jednostką.
		/// </summary>
		public string QuantityWithUnit { get; set; } = null!;

		/// <summary>
		/// Kalorie spożytego posiłku.
		/// </summary>
		public float Calories { get; set; }

		/// <summary>
		/// Czas spożycia w formacie HH:mm.
		/// </summary>
		public string ConsumedTime { get; set; } = null!;
	}
}