namespace CalorieTracker.Api.Models.Recipes
{
	/// <summary>
	/// Model transferu danych (DTO) reprezentujący wartości odżywcze.
	/// Może dotyczyć pojedynczego składnika lub całego przepisu.
	/// </summary>
	public class RecipeNutritionDto
	{
		/// <summary>
		/// Całkowita wartość kaloryczna w kcal.
		/// </summary>
		public float Calories { get; set; }

		/// <summary>
		/// Ilość białka w gramach.
		/// </summary>
		public float Protein { get; set; }

		/// <summary>
		/// Ilość tłuszczu w gramach.
		/// </summary>
		public float Fat { get; set; }

		/// <summary>
		/// Ilość węglowodanów w gramach.
		/// </summary>
		public float Carbohydrates { get; set; }

		/// <summary>
		/// Ilość błonnika w gramach (opcjonalna).
		/// </summary>
		public float? Fiber { get; set; }

		/// <summary>
		/// Ilość cukru w gramach (opcjonalna).
		/// </summary>
		public float? Sugar { get; set; }

		/// <summary>
		/// Ilość sodu w miligramach (opcjonalna).
		/// </summary>
		public float? Sodium { get; set; }
	}
}
