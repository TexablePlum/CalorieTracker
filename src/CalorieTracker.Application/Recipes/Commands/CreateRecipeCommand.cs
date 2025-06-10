namespace CalorieTracker.Application.Recipes.Commands
{
	/// <summary>
	/// Komenda do utworzenia nowego przepisu
	/// </summary>
	public record CreateRecipeCommand
	{
		public string Name { get; init; } = null!;
		public string Instructions { get; init; } = null!;
		public int ServingsCount { get; init; }
		public float TotalWeightGrams { get; init; }
		public int PreparationTimeMinutes { get; init; }
		public string CreatedByUserId { get; init; } = null!;
		public List<CreateRecipeIngredientCommand> Ingredients { get; init; } = new();
	}
}