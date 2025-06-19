// Plik RecipeNutritionCalculator.cs - serwis domenowy do kalkulacji wartości odżywczych przepisów.
// Odpowiada za obliczanie sumarycznych wartości odżywczych dla całych przepisów oraz pojedynczych składników,
// uwzględniając różne jednostki miary produktów.

using CalorieTracker.Domain.Entities;
using CalorieTracker.Domain.ValueObjects;
using CalorieTracker.Domain.Enums;
using CalorieTracker.Domain.Services.Interfaces;

namespace CalorieTracker.Domain.Services
{
	/// &lt;summary>
	/// Serwis domenowy do kalkulacji wartości odżywczych przepisów.
	/// Zawiera logikę biznesową do precyzyjnego obliczania makroskładników i kalorii na podstawie składników przepisu.
	/// &lt;/summary>
	public class RecipeNutritionCalculator : IRecipeNutritionCalculator
	{
		/// &lt;summary>
		/// Oblicza całkowitą wartość odżywczą przepisu na podstawie listy jego składników.
		/// Iteruje przez wszystkie składniki, oblicza ich indywidualne wartości odżywcze i sumuje je.
		/// &lt;/summary>
		/// &lt;param name="recipe">Obiekt &lt;see cref="Recipe"/>, dla którego mają zostać obliczone wartości odżywcze.&lt;/param>
		/// &lt;returns>Obiekt &lt;see cref="RecipeNutrition"/> zawierający sumaryczne wartości odżywcze przepisu.&lt;/returns>
		public RecipeNutrition CalculateForRecipe(Recipe recipe)
		{
			var totalNutrition = new RecipeNutrition();

			foreach (var ingredient in recipe.Ingredients)
			{
				var ingredientNutrition = CalculateIngredientNutrition(ingredient);
				totalNutrition.Add(ingredientNutrition);
			}

			return totalNutrition;
		}

		/// <summary>
		/// Oblicza wartość odżywczą pojedynczego składnika w przepisie.
		/// Przelicza wartości odżywcze produktu z "na 100g" na faktyczną ilość użytego składnika,
		/// uwzględniając jego jednostkę miary.
		/// </summary>
		/// <param name="ingredient">Obiekt <see cref="RecipeIngredient"/> reprezentujący pojedynczy składnik przepisu.</param>
		/// <returns>Obiekt <see cref="RecipeNutrition"/> zawierający wartości odżywcze dla danego składnika.</returns>
		private RecipeNutrition CalculateIngredientNutrition(RecipeIngredient ingredient)
		{
			var quantityInGrams = ConvertToGrams(ingredient);
			var product = ingredient.Product;

			// Współczynnik przeliczeniowy z "na 100g" na faktyczną ilość w gramach
			var factor = quantityInGrams / 100f;

			return new RecipeNutrition(
				calories: product.CaloriesPer100g * factor,
				protein: product.ProteinPer100g * factor,
				fat: product.FatPer100g * factor,
				carbohydrates: product.CarbohydratesPer100g * factor,
				fiber: product.FiberPer100g * factor,
				sugar: product.SugarsPer100g * factor,
				sodium: product.SodiumPer100g * factor
			);
		}

		/// <summary>
		/// Konwertuje ilość składnika na gramy, w zależności od jednostki miary produktu.
		/// Dla jednostki <see cref="ProductUnit.Piece"/> używa <see cref="Product.ServingSize"/> produktu.
		/// Dla <see cref="ProductUnit.Milliliter"/> zakłada gęstość zbliżoną do wody (1ml ~ 1g).
		/// </summary>
		/// <param name="ingredient">Obiekt <see cref="RecipeIngredient"/> do konwersji.</param>
		/// <returns>Ilość składnika w gramach.</returns>
		/// <exception cref="ArgumentException">Wyrzucany, gdy napotka nieobsługiwaną jednostkę produktu.</exception>
		private float ConvertToGrams(RecipeIngredient ingredient)
		{
			return ingredient.Product.Unit switch
			{
				ProductUnit.Piece => ingredient.Quantity * ingredient.Product.ServingSize,
				ProductUnit.Gram => ingredient.Quantity,
				ProductUnit.Milliliter => ingredient.Quantity, // Założenie: 1ml ~ 1g dla większości płynów
				_ => throw new ArgumentException($"Nieobsługiwana jednostka: {ingredient.Product.Unit}")
			};
		}
	}
}