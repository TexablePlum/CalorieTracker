// Plik MealNutritionDto.cs - DTO wartości odżywczych posiłku.

namespace CalorieTracker.Api.Models.NutritionTracking
{
	/// <summary>
	/// Model transferu danych reprezentujący wartości odżywcze pojedynczego posiłku.
	/// </summary>
	public class MealNutritionDto
	{
		/// <summary>
		/// Kalorie w kcal.
		/// </summary>
		public float Calories { get; set; }

		/// <summary>
		/// Białko w gramach.
		/// </summary>
		public float Protein { get; set; }

		/// <summary>
		/// Tłuszcze w gramach.
		/// </summary>
		public float Fat { get; set; }

		/// <summary>
		/// Węglowodany w gramach.
		/// </summary>
		public float Carbohydrates { get; set; }

		/// <summary>
		/// Błonnik w gramach (opcjonalnie).
		/// </summary>
		public float? Fiber { get; set; }

		/// <summary>
		/// Cukier w gramach (opcjonalnie).
		/// </summary>
		public float? Sugar { get; set; }

		/// <summary>
		/// Sód w miligramach (opcjonalnie).
		/// </summary>
		public float? Sodium { get; set; }
	}
}