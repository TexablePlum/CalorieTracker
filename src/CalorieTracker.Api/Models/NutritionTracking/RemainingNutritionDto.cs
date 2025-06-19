// Plik RemainingNutritionDto.cs - DTO pozostałych potrzeb żywieniowych.

namespace CalorieTracker.Api.Models.NutritionTracking
{
	/// <summary>
	/// Model transferu danych reprezentujący pozostałe do spożycia wartości odżywcze.
	/// </summary>
	public class RemainingNutritionDto
	{
		/// <summary>
		/// Pozostałe kalorie do spożycia.
		/// </summary>
		public float Calories { get; set; }

		/// <summary>
		/// Pozostały przedział białka do spożycia.
		/// </summary>
		public MacronutrientRangeDto Protein { get; set; } = null!;

		/// <summary>
		/// Pozostały przedział tłuszczów do spożycia.
		/// </summary>
		public MacronutrientRangeDto Fat { get; set; } = null!;

		/// <summary>
		/// Pozostały przedział węglowodanów do spożycia.
		/// </summary>
		public MacronutrientRangeDto Carbohydrates { get; set; } = null!;

		/// <summary>
		/// Pozostała woda do wypicia w litrach.
		/// </summary>
		public float WaterLiters { get; set; }
	}
}