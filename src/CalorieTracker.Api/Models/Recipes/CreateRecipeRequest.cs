namespace CalorieTracker.Api.Models.Recipes
{
	/// <summary>
	/// DTO do tworzenia przepisu
	/// </summary>
	public class CreateRecipeRequest
	{
		public string Name { get; set; } = null!;
		public string Instructions { get; set; } = null!;
		public int ServingsCount { get; set; }
		public float TotalWeightGrams { get; set; }
		public int PreparationTimeMinutes { get; set; }
		public List<CreateRecipeIngredientRequest> Ingredients { get; set; } = new();
	}

	/// <summary>
	/// DTO składnika podczas tworzenia przepisu
	/// </summary>
	public class CreateRecipeIngredientRequest
	{
		public Guid ProductId { get; set; }
		public float Quantity { get; set; }
	}

	/// <summary>
	/// DTO do aktualizacji przepisu
	/// </summary>
	public class UpdateRecipeRequest
	{
		public string Name { get; set; } = null!;
		public string Instructions { get; set; } = null!;
		public int ServingsCount { get; set; }
		public float TotalWeightGrams { get; set; }
		public int PreparationTimeMinutes { get; set; }
		public List<CreateRecipeIngredientRequest> Ingredients { get; set; } = new();
	}

	/// <summary>
	/// DTO do wyszukiwania przepisów
	/// </summary>
	public class SearchRecipesRequest
	{
		public string SearchTerm { get; set; } = null!;
		public int Skip { get; set; } = 0;
		public int Take { get; set; } = 20;
	}

	/// <summary>
	/// Response dla wyszukiwania z paginacją
	/// </summary>
	public class SearchRecipesResponse
	{
		public List<RecipeSummaryDto> Recipes { get; set; } = new();
		public int TotalCount { get; set; }
		public bool HasMore { get; set; }
	}
}