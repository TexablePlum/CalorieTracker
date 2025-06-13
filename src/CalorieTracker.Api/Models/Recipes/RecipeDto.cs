namespace CalorieTracker.Api.Models.Recipes
{
	/// <summary>
	/// DTO zwracane przez API ze szczegółami przepisu
	/// </summary>
	public class RecipeDto
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = null!;
		public string Instructions { get; set; } = null!;
		public int ServingsCount { get; set; }
		public float TotalWeightGrams { get; set; }
		public int PreparationTimeMinutes { get; set; }
		public string CreatedByUserId { get; set; } = null!;
		public DateTime CreatedAt { get; set; }

		public List<RecipeIngredientDto> Ingredients { get; set; } = new();
		public RecipeNutritionDto TotalNutrition { get; set; } = null!;
	}

	/// <summary>
	/// DTO składnika w przepisie
	/// </summary>
	public class RecipeIngredientDto
	{
		public Guid ProductId { get; set; }
		public string ProductName { get; set; } = null!;
		public float Quantity { get; set; }
		public string Unit { get; set; } = null!;
	}

	/// <summary>
	/// DTO wartości odżywczych przepisu
	/// </summary>
	public class RecipeNutritionDto
	{
		public float Calories { get; set; }
		public float Protein { get; set; }
		public float Fat { get; set; }
		public float Carbohydrates { get; set; }
		public float? Fiber { get; set; }
		public float? Sugar { get; set; }
		public float? Sodium { get; set; }
	}

	/// <summary>
	/// Uproszczone DTO przepisu dla list
	/// </summary>
	public class RecipeSummaryDto
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = null!;
		public int ServingsCount { get; set; }
		public float TotalWeightGrams { get; set; }
		public int PreparationTimeMinutes { get; set; }
		public float TotalCalories { get; set; }
		public string CreatedByUserId { get; set; } = null!;
		public DateTime CreatedAt { get; set; }
	}
}