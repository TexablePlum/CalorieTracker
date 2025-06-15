// Plik GetRecipeDetailsQuery.cs - definicja zapytania o szczegóły przepisu.
// Reprezentuje żądanie pobrania pełnych informacji o konkretnym przepisie.

namespace CalorieTracker.Application.Recipes.Queries
{
	/// <summary>
	/// Rekord reprezentujący zapytanie o szczegóły przepisu.
	/// Przenosi identyfikator przepisu, dla którego mają zostać zwrócone dane.
	/// </summary>
	/// <param name="Id">Unikalny identyfikator przepisu do wyszukania</param>
	public record GetRecipeDetailsQuery(Guid Id);
}