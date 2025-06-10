

namespace CalorieTracker.Application.Recipes.Commands
{
	/// <summary>
	/// Składnik w komendzie tworzenia przepisu
	/// </summary>
	public record CreateRecipeIngredientCommand
	{
		public Guid ProductId { get; init; }
		public float Quantity { get; init; }
	}
}
