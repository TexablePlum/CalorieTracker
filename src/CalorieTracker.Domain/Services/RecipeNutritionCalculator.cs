using CalorieTracker.Domain.Entities;
using CalorieTracker.Domain.ValueObjects;
using CalorieTracker.Domain.Enums;

namespace CalorieTracker.Domain.Services
{
	/// <summary>
	/// Serwis domenowy do kalkulacji wartości odżywczych przepisów
	/// </summary>
	public class RecipeNutritionCalculator
	{
		/// <summary>
		/// Oblicza całkowitą wartość odżywczą przepisu na podstawie składników
		/// </summary>
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
		/// Oblicza wartość odżywczą pojedynczego składnika w przepisie
		/// </summary>
		private RecipeNutrition CalculateIngredientNutrition(RecipeIngredient ingredient)
		{
			var quantityInGrams = ConvertToGrams(ingredient);
			var product = ingredient.Product;

			// Przelicz wartości odżywcze z "na 100g" na faktyczną ilość
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
		/// Konwertuje ilość składnika na gramy w zależności od jednostki produktu
		/// </summary>
		private float ConvertToGrams(RecipeIngredient ingredient)
		{
			return ingredient.Product.Unit switch
			{
				ProductUnit.Piece => ingredient.Quantity * ingredient.Product.ServingSize,
				ProductUnit.Gram => ingredient.Quantity,
				ProductUnit.Milliliter => ingredient.Quantity, // 1ml ~ 1g dla większości płynów
				_ => throw new ArgumentException($"Nieobsługiwana jednostka: {ingredient.Product.Unit}")
			};
		}
	}
}