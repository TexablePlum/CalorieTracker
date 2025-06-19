// Plik ConsumedNutritionDto.cs - DTO spożytych wartości odżywczych.

namespace CalorieTracker.Api.Models.NutritionTracking
{
	/// <summary>
	/// Model transferu danych reprezentujący rzeczywiste spożyte wartości odżywcze.
	/// </summary>
	public class ConsumedNutritionDto
	{
		/// <summary>
		/// Spożyte kalorie w kcal.
		/// </summary>
		public float Calories { get; set; }

		/// <summary>
		/// Spożyte białko w gramach.
		/// </summary>
		public float Protein { get; set; }

		/// <summary>
		/// Spożyte tłuszcze w gramach.
		/// </summary>
		public float Fat { get; set; }

		/// <summary>
		/// Spożyte węglowodany w gramach.
		/// </summary>
		public float Carbohydrates { get; set; }

		/// <summary>
		/// Wypita woda w litrach.
		/// </summary>
		public float WaterLiters { get; set; }

		/// <summary>
		/// Spożyty błonnik w gramach (opcjonalnie).
		/// </summary>
		public float? Fiber { get; set; }

		/// <summary>
		/// Spożyty cukier w gramach (opcjonalnie).
		/// </summary>
		public float? Sugar { get; set; }

		/// <summary>
		/// Spożyty sód w miligramach (opcjonalnie).
		/// </summary>
		public float? Sodium { get; set; }
	}
}