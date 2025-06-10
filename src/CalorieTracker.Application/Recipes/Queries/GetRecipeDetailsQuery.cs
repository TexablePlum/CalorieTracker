namespace CalorieTracker.Application.Recipes.Queries
{
	/// <summary>
	/// Query do pobrania szczegółów przepisu po ID
	/// </summary>
	public record GetRecipeDetailsQuery(Guid Id);
}