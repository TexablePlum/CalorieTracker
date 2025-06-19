// Plik NutritionGoalsDto.cs - DTO celów żywieniowych.

namespace CalorieTracker.Api.Models.NutritionTracking
{
	/// <summary>
	/// Model transferu danych reprezentujący dzienne cele żywieniowe użytkownika.
	/// </summary>
	public class NutritionGoalsDto
	{
		/// <summary>
		/// Docelowe dzienne kalorie.
		/// </summary>
		public float Calories { get; set; }

		/// <summary>
		/// Przedział dziennego celu białkowego.
		/// </summary>
		public MacronutrientRangeDto Protein { get; set; } = null!;

		/// <summary>
		/// Przedział dziennego celu tłuszczowego.
		/// </summary>
		public MacronutrientRangeDto Fat { get; set; } = null!;

		/// <summary>
		/// Przedział dziennego celu węglowodanowego.
		/// </summary>
		public MacronutrientRangeDto Carbohydrates { get; set; } = null!;

		/// <summary>
		/// Dzienny cel wodny w litrach.
		/// </summary>
		public float WaterLiters { get; set; }
	}
}