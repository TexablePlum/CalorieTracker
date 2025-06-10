namespace CalorieTracker.Application.Recipes.Commands
{
	/// <summary>
	/// Komenda do aktualizacji przepisu
	/// </summary>
	public record UpdateRecipeCommand
	{
		public Guid Id { get; init; }
		public string Name { get; init; } = null!;
		public string Instructions { get; init; } = null!;
		public int ServingsCount { get; init; }
		public float TotalWeightGrams { get; init; }
		public int PreparationTimeMinutes { get; init; }
		public string UpdatedByUserId { get; init; } = null!;
		public List<CreateRecipeIngredientCommand> Ingredients { get; init; } = new();
	}
}