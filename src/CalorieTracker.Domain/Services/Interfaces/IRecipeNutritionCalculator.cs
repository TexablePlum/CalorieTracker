// Plik IRecipeNutritionCalculator.cs - interfejs dla serwisu kalkulacji wartości odżywczych przepisów.

using CalorieTracker.Domain.Entities;
using CalorieTracker.Domain.ValueObjects;

namespace CalorieTracker.Domain.Services.Interfaces
{
	/// <summary>
	/// Interfejs dla serwisu domenowego do kalkulacji wartości odżywczych przepisów.
	/// Pozwala na mockowanie w testach jednostkowych.
	/// </summary>
	public interface IRecipeNutritionCalculator
	{
		/// <summary>
		/// Oblicza całkowitą wartość odżywczą przepisu na podstawie listy jego składników.
		/// </summary>
		/// <param name="recipe">Obiekt Recipe, dla którego mają zostać obliczone wartości odżywcze.</param>
		/// <returns>Obiekt RecipeNutrition zawierający sumaryczne wartości odżywcze przepisu.</returns>
		RecipeNutrition CalculateForRecipe(Recipe recipe);
	}
}
