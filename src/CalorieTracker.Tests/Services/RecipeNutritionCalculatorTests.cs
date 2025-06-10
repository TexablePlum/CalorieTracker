using CalorieTracker.Domain.Entities;
using CalorieTracker.Domain.Enums;
using CalorieTracker.Domain.Services;
using Xunit;

namespace Tests.Services
{
	public class RecipeNutritionCalculatorTests
	{
		private readonly RecipeNutritionCalculator _calculator;

		public RecipeNutritionCalculatorTests()
		{
			_calculator = new RecipeNutritionCalculator();
		}

		[Fact]
		public void CalculateForRecipe_WithGramBasedProduct_ShouldCalculateCorrectly()
		{
			// Arrange
			var maslo = CreateProduct("Masło", ProductUnit.Gram, 717f, 0.9f, 81f, 0.1f, servingSize: 100f);
			var recipe = CreateRecipe("Test Recipe");
			recipe.Ingredients.Add(CreateIngredient(recipe.Id, maslo, 50f)); // 50g masła

			// Act
			var result = _calculator.CalculateForRecipe(recipe);

			// Assert
			Assert.Equal(358.5f, result.Calories, 1); // 50g * 717 kcal/100g = 358.5
			Assert.Equal(0.45f, result.Protein, 2);   // 50g * 0.9g/100g = 0.45g
			Assert.Equal(40.5f, result.Fat, 1);       // 50g * 81g/100g = 40.5g
			Assert.Equal(0.05f, result.Carbohydrates, 2); // 50g * 0.1g/100g = 0.05g
		}

		[Fact]
		public void CalculateForRecipe_WithPieceBasedProduct_ShouldCalculateCorrectly()
		{
			// Arrange - jajko: 1 sztuka = 50g, 155 kcal/100g
			var jajko = CreateProduct("Jajko", ProductUnit.Piece, 155f, 13f, 11f, 1.1f, servingSize: 50f);
			var recipe = CreateRecipe("Jajecznica");
			recipe.Ingredients.Add(CreateIngredient(recipe.Id, jajko, 2f)); // 2 jajka

			// Act
			var result = _calculator.CalculateForRecipe(recipe);

			// Assert - 2 jajka * 50g = 100g = 1 * wartości per 100g
			Assert.Equal(155f, result.Calories);
			Assert.Equal(13f, result.Protein);
			Assert.Equal(11f, result.Fat);
			Assert.Equal(1.1f, result.Carbohydrates);
		}

		[Fact]
		public void CalculateForRecipe_WithMilliliterBasedProduct_ShouldCalculateCorrectly()
		{
			// Arrange - mleko: 100ml, 42 kcal/100ml
			var mleko = CreateProduct("Mleko", ProductUnit.Milliliter, 42f, 3.4f, 1f, 4.8f, servingSize: 100f);
			var recipe = CreateRecipe("Koktajl");
			recipe.Ingredients.Add(CreateIngredient(recipe.Id, mleko, 250f)); // 250ml mleka

			// Act
			var result = _calculator.CalculateForRecipe(recipe);

			// Assert - 250ml * 42 kcal/100ml = 105 kcal
			Assert.Equal(105f, result.Calories);
			Assert.Equal(8.5f, result.Protein, 1);   // 250 * 3.4/100 = 8.5
			Assert.Equal(2.5f, result.Fat, 1);       // 250 * 1/100 = 2.5
			Assert.Equal(12f, result.Carbohydrates); // 250 * 4.8/100 = 12
		}

		[Fact]
		public void CalculateForRecipe_WithFractionalQuantity_ShouldCalculateCorrectly()
		{
			// Arrange - pół jajka
			var jajko = CreateProduct("Jajko", ProductUnit.Piece, 155f, 13f, 11f, 1.1f, servingSize: 50f);
			var recipe = CreateRecipe("Mini omlet");
			recipe.Ingredients.Add(CreateIngredient(recipe.Id, jajko, 0.5f)); // 0.5 jajka

			// Act
			var result = _calculator.CalculateForRecipe(recipe);

			// Assert - 0.5 jajka * 50g = 25g = 0.25 * wartości per 100g
			Assert.Equal(38.75f, result.Calories, 2); // 25g * 155/100 = 38.75
			Assert.Equal(3.25f, result.Protein, 2);   // 25g * 13/100 = 3.25
			Assert.Equal(2.75f, result.Fat, 2);       // 25g * 11/100 = 2.75
			Assert.Equal(0.275f, result.Carbohydrates, 3); // 25g * 1.1/100 = 0.275
		}

		[Fact]
		public void CalculateForRecipe_WithMultipleIngredients_ShouldSumCorrectly()
		{
			// Arrange
			var jajko = CreateProduct("Jajko", ProductUnit.Piece, 155f, 13f, 11f, 1.1f, servingSize: 50f);
			var maslo = CreateProduct("Masło", ProductUnit.Gram, 717f, 0.9f, 81f, 0.1f, servingSize: 100f);
			var mleko = CreateProduct("Mleko", ProductUnit.Milliliter, 42f, 3.4f, 1f, 4.8f, servingSize: 100f);

			var recipe = CreateRecipe("Omlet");
			recipe.Ingredients.Add(CreateIngredient(recipe.Id, jajko, 2f));  // 2 jajka (100g)
			recipe.Ingredients.Add(CreateIngredient(recipe.Id, maslo, 10f)); // 10g masła
			recipe.Ingredients.Add(CreateIngredient(recipe.Id, mleko, 50f)); // 50ml mleka

			// Act
			var result = _calculator.CalculateForRecipe(recipe);

			// Assert
			// 2 jajka: 155 kcal + 10g masła: 71.7 kcal + 50ml mleka: 21 kcal = 247.7 kcal
			Assert.Equal(247.7f, result.Calories, 1);

			// Białko: 13g + 0.09g + 1.7g = 14.79g
			Assert.Equal(14.79f, result.Protein, 2);

			// Tłuszcze: 11g + 8.1g + 0.5g = 19.6g
			Assert.Equal(19.6f, result.Fat, 1);

			// Węglowodany: 1.1g + 0.01g + 2.4g = 3.51g
			Assert.Equal(3.51f, result.Carbohydrates, 2);
		}

		[Fact]
		public void CalculateForRecipe_WithOptionalNutrients_ShouldHandleNullValues()
		{
			// Arrange - produkt z niektórymi wartościami null
			var produkt = new Product
			{
				Id = Guid.NewGuid(),
				Name = "Test Product",
				Unit = ProductUnit.Gram,
				ServingSize = 100f,
				CaloriesPer100g = 100f,
				ProteinPer100g = 10f,
				FatPer100g = 5f,
				CarbohydratesPer100g = 15f,
				FiberPer100g = 2f,
				SugarsPer100g = null,
				SodiumPer100g = 500f
			};

			var recipe = CreateRecipe("Test Recipe");
			recipe.Ingredients.Add(CreateIngredient(recipe.Id, produkt, 50f)); // 50g

			// Act
			var result = _calculator.CalculateForRecipe(recipe);

			// Assert
			Assert.Equal(50f, result.Calories);
			Assert.Equal(5f, result.Protein);
			Assert.Equal(2.5f, result.Fat);
			Assert.Equal(7.5f, result.Carbohydrates);
			Assert.Equal(1f, result.Fiber);     // 50g * 2g/100g = 1g
			Assert.Null(result.Sugar);          // Powinno zostać null
			Assert.Equal(250f, result.Sodium);  // 50g * 500mg/100g = 250mg
		}

		[Fact]
		public void CalculateForRecipe_WithEmptyIngredients_ShouldReturnZeroValues()
		{
			// Arrange
			var recipe = CreateRecipe("Empty Recipe");
			// Brak składników

			// Act
			var result = _calculator.CalculateForRecipe(recipe);

			// Assert
			Assert.Equal(0f, result.Calories);
			Assert.Equal(0f, result.Protein);
			Assert.Equal(0f, result.Fat);
			Assert.Equal(0f, result.Carbohydrates);
			Assert.Null(result.Fiber);
			Assert.Null(result.Sugar);
			Assert.Null(result.Sodium);
		}

		[Theory]
		[InlineData(ProductUnit.Gram, 100f, 100f)]    // 100g produktu = 100g
		[InlineData(ProductUnit.Milliliter, 200f, 200f)] // 200ml produktu = 200g (1:1)
		[InlineData(ProductUnit.Piece, 3f, 150f)]     // 3 sztuki po 50g = 150g
		public void ConvertToGrams_ShouldCalculateCorrectWeight(ProductUnit unit, float quantity, float expectedGrams)
		{
			// Arrange
			var product = CreateProduct("Test", unit, 100f, 10f, 5f, 15f, servingSize: 50f);
			var recipe = CreateRecipe("Test");
			recipe.Ingredients.Add(CreateIngredient(recipe.Id, product, quantity));

			// Act
			var result = _calculator.CalculateForRecipe(recipe);

			// Assert - sprawdzamy czy kalorie są proporcjonalne do masy
			var expectedCalories = expectedGrams * 100f / 100f; // 100 kcal/100g
			Assert.Equal(expectedCalories, result.Calories);
		}

		// Helper methods
		private Product CreateProduct(string name, ProductUnit unit, float calories, float protein,
			float fat, float carbs, float servingSize = 100f)
		{
			return new Product
			{
				Id = Guid.NewGuid(),
				Name = name,
				Unit = unit,
				ServingSize = servingSize,
				CaloriesPer100g = calories,
				ProteinPer100g = protein,
				FatPer100g = fat,
				CarbohydratesPer100g = carbs
			};
		}

		private Recipe CreateRecipe(string name)
		{
			return new Recipe
			{
				Id = Guid.NewGuid(),
				Name = name,
				Instructions = "Test instructions",
				ServingsCount = 1,
				TotalWeightGrams = 100f,
				PreparationTimeMinutes = 10,
				CreatedByUserId = "test-user",
				Ingredients = new List<RecipeIngredient>()
			};
		}

		private RecipeIngredient CreateIngredient(Guid recipeId, Product product, float quantity)
		{
			return new RecipeIngredient
			{
				Id = Guid.NewGuid(),
				RecipeId = recipeId,
				ProductId = product.Id,
				Quantity = quantity,
				Product = product
			};
		}
	}
}
