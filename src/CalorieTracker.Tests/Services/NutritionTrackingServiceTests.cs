// Plik NutritionTrackingServiceTests.cs - testy jednostkowe serwisu śledzenia żywienia.
// Weryfikuje poprawność kalkulacji wartości odżywczych i agregacji danych nutricyjnych.

using CalorieTracker.Domain.Entities;
using CalorieTracker.Domain.Enums;
using CalorieTracker.Domain.Services;
using CalorieTracker.Domain.Services.Interfaces;
using CalorieTracker.Domain.ValueObjects;
using Moq;
using Xunit;

namespace CalorieTracker.Tests.Services
{
	/// <summary>
	/// Testy jednostkowe dla serwisu śledzenia żywienia.
	/// Weryfikuje poprawność kalkulacji nutrition dla posiłków oraz agregacji dziennego spożycia.
	/// Używa mocków dla zależności w celu izolacji testowanej funkcjonalności.
	/// </summary>
	public class NutritionTrackingServiceTests
	{
		/// <summary>
		/// Mock abstrakcji serwisu kalkulacji przepisów.
		/// </summary>
		private readonly Mock<IRecipeNutritionCalculator> _mockRecipeCalculator;

		/// <summary>
		/// Mock serwisu kalkulacji wymagań żywieniowych.
		/// </summary>
		private readonly Mock<NutritionCalculationService> _mockNutritionCalculation;

		/// <summary>
		/// Testowana instancja serwisu śledzenia żywienia.
		/// </summary>
		private readonly NutritionTrackingService _service;

		/// <summary>
		/// Inicjalizuje nową instancję testów z zamockowanymi zależnościami.
		/// </summary>
		public NutritionTrackingServiceTests()
		{
			_mockRecipeCalculator = new Mock<IRecipeNutritionCalculator>();
			_mockNutritionCalculation = new Mock<NutritionCalculationService>();
			_service = new NutritionTrackingService(_mockRecipeCalculator.Object, _mockNutritionCalculation.Object);
		}

		#region CalculateNutritionForMealEntry Tests

		/// <summary>
		/// Weryfikuje poprawność kalkulacji wartości odżywczych dla produktu w gramach.
		/// Test sprawdza podstawową konwersję z "na 100g" na rzeczywistą spożytą ilość.
		/// </summary>
		[Fact]
		public void CalculateNutritionForMealEntry_ProductInGrams_ShouldCalculateCorrectly()
		{
			// Arrange - produkt w gramach
			var product = CreateProduct("Kurczak", ProductUnit.Gram, 165f, 25f, 3.6f, 0f);
			var mealEntry = CreateMealLogEntry(productId: Guid.NewGuid(), quantity: 150f);
			mealEntry.Product = product;

			// Act - kalkulacja wartości odżywczych
			_service.CalculateNutritionForMealEntry(mealEntry);

			// Assert - sprawdzenie przeliczenia (150g / 100g = 1.5x)
			Assert.Equal(247.5f, mealEntry.CalculatedCalories, 1); // 165 * 1.5
			Assert.Equal(37.5f, mealEntry.CalculatedProtein, 1);   // 25 * 1.5
			Assert.Equal(5.4f, mealEntry.CalculatedFat, 1);        // 3.6 * 1.5
			Assert.Equal(0f, mealEntry.CalculatedCarbohydrates, 1); // 0 * 1.5
		}

		/// <summary>
		/// Weryfikuje poprawność kalkulacji dla produktu w sztukach.
		/// Test sprawdza konwersję sztuk na gramy przez ServingSize.
		/// </summary>
		[Fact]
		public void CalculateNutritionForMealEntry_ProductInPieces_ShouldUseServingSize()
		{
			// Arrange - produkt w sztukach (jajko = 60g/szt)
			var product = CreateProduct("Jajka", ProductUnit.Piece, 155f, 13f, 11f, 1.1f, servingSize: 60f);
			var mealEntry = CreateMealLogEntry(productId: Guid.NewGuid(), quantity: 2f); // 2 jajka
			mealEntry.Product = product;

			// Act - kalkulacja
			_service.CalculateNutritionForMealEntry(mealEntry);

			// Assert - 2 sztuki * 60g = 120g, factor = 1.2
			Assert.Equal(186f, mealEntry.CalculatedCalories, 1);		// 155 * 1.2
			Assert.Equal(15.6f, mealEntry.CalculatedProtein, 1);		// 13 * 1.2
			Assert.Equal(13.2f, mealEntry.CalculatedFat, 1);			// 11 * 1.2
			Assert.Equal(1.3f, mealEntry.CalculatedCarbohydrates, 1);	// 1.1 * 1.2
		}

		/// <summary>
		/// Weryfikuje poprawność kalkulacji dla produktu w mililitrach.
		/// Test sprawdza założenie 1ml ≈ 1g dla płynów.
		/// </summary>
		[Fact]
		public void CalculateNutritionForMealEntry_ProductInMilliliters_ShouldTreatAsGrams()
		{
			// Arrange - mleko w mililitrach
			var product = CreateProduct("Mleko", ProductUnit.Milliliter, 64f, 3.2f, 3.6f, 4.8f);
			var mealEntry = CreateMealLogEntry(productId: Guid.NewGuid(), quantity: 250f); // 250ml
			mealEntry.Product = product;

			// Act - kalkulacja (250ml ≈ 250g)
			_service.CalculateNutritionForMealEntry(mealEntry);

			// Assert - factor = 2.5
			Assert.Equal(160f, mealEntry.CalculatedCalories, 1);		// 64 * 2.5
			Assert.Equal(8f, mealEntry.CalculatedProtein, 1);			// 3.2 * 2.5
			Assert.Equal(9f, mealEntry.CalculatedFat, 1);				// 3.6 * 2.5
			Assert.Equal(12f, mealEntry.CalculatedCarbohydrates, 1);	// 4.8 * 2.5
		}

		/// <summary>
		/// Weryfikuje kalkulację wartości odżywczych dla przepisu kulinarnego.
		/// Test sprawdza użycie IRecipeNutritionCalculator i skalowanie do porcji.
		/// </summary>
		[Fact]
		public void CalculateNutritionForMealEntry_Recipe_ShouldUseRecipeCalculator()
		{
			// Arrange - przepis z mockiem kalkulatora
			var recipe = CreateRecipe("Sałatka owocowa");
			var mealEntry = CreateMealLogEntry(recipeId: Guid.NewGuid(), quantity: 150f);
			mealEntry.Recipe = recipe;

			// Setup mock - całkowite wartości przepisu
			var recipeNutrition = new RecipeNutrition(320f, 8f, 12f, 65f, 15f, 45f, 150f);
			_mockRecipeCalculator.Setup(x => x.CalculateForRecipe(recipe))
								 .Returns(recipeNutrition);

			// Act - kalkulacja
			_service.CalculateNutritionForMealEntry(mealEntry);

			// Assert - sprawdzenie skalowania (150g / 100g = 1.5x)
			Assert.Equal(480f, mealEntry.CalculatedCalories, 1);		// 320 * 1.5
			Assert.Equal(12f, mealEntry.CalculatedProtein, 1);			// 8 * 1.5
			Assert.Equal(18f, mealEntry.CalculatedFat, 1);				// 12 * 1.5
			Assert.Equal(97.5f, mealEntry.CalculatedCarbohydrates, 1);	// 65 * 1.5
			Assert.Equal(22.5f, mealEntry.CalculatedFiber!.Value, 1);	// 15 * 1.5
			Assert.Equal(67.5f, mealEntry.CalculatedSugar!.Value, 1);	// 45 * 1.5
			Assert.Equal(225f, mealEntry.CalculatedSodium!.Value, 1);	// 150 * 1.5

			// Weryfikacja wywołania mocka
			_mockRecipeCalculator.Verify(x => x.CalculateForRecipe(recipe), Times.Once);
		}

		/// <summary>
		/// Weryfikuje obsługę opcjonalnych składników odżywczych (fiber, sugar, sodium).
		/// Test sprawdza czy null values są poprawnie przetwarzane.
		/// </summary>
		[Fact]
		public void CalculateNutritionForMealEntry_ProductWithOptionalNutrients_ShouldHandleNulls()
		{
			// Arrange - produkt z niektórymi wartościami null
			var product = CreateProduct("Produkt testowy", ProductUnit.Gram, 100f, 10f, 5f, 15f);
			product.FiberPer100g = 3f;     // Ma błonnik
			product.SugarsPer100g = null;  // Brak cukru
			product.SodiumPer100g = 200f;  // Ma sód

			var mealEntry = CreateMealLogEntry(productId: Guid.NewGuid(), quantity: 200f);
			mealEntry.Product = product;

			// Act - kalkulacja
			_service.CalculateNutritionForMealEntry(mealEntry);

			// Assert - sprawdzenie obsługi null values
			Assert.Equal(6f, mealEntry.CalculatedFiber!.Value, 1);		// 3 * 2
			Assert.Null(mealEntry.CalculatedSugar);						// Pozostaje null
			Assert.Equal(400f, mealEntry.CalculatedSodium!.Value, 1);	// 200 * 2
		}

		#endregion

		#region AggregateConsumedNutrition Tests

		/// <summary>
		/// Weryfikuje poprawność agregacji wartości odżywczych z kilku posiłków.
		/// Test sprawdza sumowanie wszystkich makroskładników z różnych wpisów.
		/// </summary>
		[Fact]
		public void AggregateConsumedNutrition_MultipleEntries_ShouldSumCorrectly()
		{
			// Arrange - lista różnych posiłków
			var meals = new List<MealLogEntry>
			{
				CreateCalculatedMealEntry(250f, 20f, 10f, 30f), // Śniadanie
                CreateCalculatedMealEntry(400f, 25f, 15f, 45f), // Obiad
                CreateCalculatedMealEntry(150f, 8f, 5f, 20f)    // Przekąska
            };

			// Act - agregacja
			var result = _service.AggregateConsumedNutrition(meals, waterIntakeLiters: 2.5f);

			// Assert - sprawdzenie sum
			Assert.Equal(800f, result.Calories, 1);      // 250 + 400 + 150
			Assert.Equal(53f, result.Protein, 1);        // 20 + 25 + 8
			Assert.Equal(30f, result.Fat, 1);            // 10 + 15 + 5
			Assert.Equal(95f, result.Carbohydrates, 1);  // 30 + 45 + 20
			Assert.Equal(2.5f, result.WaterLiters, 1);
		}

		/// <summary>
		/// Weryfikuje agregację pustej listy posiłków.
		/// Test sprawdza czy funkcja poprawnie obsługuje brak danych.
		/// </summary>
		[Fact]
		public void AggregateConsumedNutrition_EmptyList_ShouldReturnZeros()
		{
			// Arrange - pusta lista
			var meals = new List<MealLogEntry>();

			// Act - agregacja pustej listy
			var result = _service.AggregateConsumedNutrition(meals, waterIntakeLiters: 1.5f);

			// Assert - wszystko powinno być zero oprócz wody
			Assert.Equal(0f, result.Calories);
			Assert.Equal(0f, result.Protein);
			Assert.Equal(0f, result.Fat);
			Assert.Equal(0f, result.Carbohydrates);
			Assert.Equal(1.5f, result.WaterLiters);
			Assert.Null(result.Fiber);
			Assert.Null(result.Sugar);
			Assert.Null(result.Sodium);
		}

		#endregion

		#region CreateMealSummaries Tests

		/// <summary>
		/// Weryfikuje tworzenie podsumowań posiłków z sortowaniem chronologicznym.
		/// Test sprawdza NAZWY PRODUKTÓW, nie nazwy typów posiłków.
		/// </summary>
		[Fact]
		public void CreateMealSummaries_MultipleMeals_ShouldCreateSortedSummaries()
		{
			// Arrange - posiłki z różnymi produktami w różnych godzinach
			var meals = new List<MealLogEntry>
			{
				// "Makaron", Dinner (20:00)
				CreateMealWithTime("Makaron", DateTime.Today.AddHours(20), 300f, MealType.Dinner),
        
				// "Płatki owsiane", Breakfast (8:00)  
				CreateMealWithTime("Płatki owsiane", DateTime.Today.AddHours(8), 250f, MealType.Breakfast),
        
				// "Kanapka", Lunch (14:00)
				CreateMealWithTime("Kanapka", DateTime.Today.AddHours(14), 450f, MealType.Lunch)
			};

			// Act - tworzenie podsumowań
			var summaries = _service.CreateMealSummaries(meals);

			// Assert - sprawdzenie sortowania chronologicznego i nazw
			Assert.Equal(3, summaries.Count);

			// 8:00 - najwcześniej
			Assert.Equal("Płatki owsiane", summaries[0].Name);
			Assert.Equal("08:00", summaries[0].ConsumedTime);
			Assert.Equal(MealType.Breakfast, summaries[0].MealType);

			// 14:00 - w środku  
			Assert.Equal("Kanapka", summaries[1].Name);
			Assert.Equal("14:00", summaries[1].ConsumedTime);
			Assert.Equal(MealType.Lunch, summaries[1].MealType);

			// 20:00 - najpóźniej
			Assert.Equal("Makaron", summaries[2].Name);  
			Assert.Equal("20:00", summaries[2].ConsumedTime);
			Assert.Equal(MealType.Dinner, summaries[2].MealType);
		}


		#endregion

		#region Validation Tests

		/// <summary>
		/// Weryfikuje walidację poprawnego wpisu posiłku.
		/// Test sprawdza czy prawidłowe dane przechodzą walidację.
		/// </summary>
		[Fact]
		public void ValidateMealLogEntry_ValidEntry_ShouldReturnTrue()
		{
			// Arrange
			var entry = CreateMealLogEntry(productId: Guid.NewGuid(), quantity: 100f);
			entry.UserId = "valid-user-id";
			entry.ConsumedAt = DateTime.UtcNow.AddMinutes(-30); // 30 min temu

			// Act & Assert
			Assert.True(_service.ValidateMealLogEntry(entry));
		}

		/// <summary>
		/// Weryfikuje walidację niepoprawnych wpisów posiłków.
		/// Test parametryzowany sprawdzający różne przypadki błędnych danych.
		/// </summary>
		/// <param name="userId">ID użytkownika do testowania.</param>
		/// <param name="quantity">Ilość do testowania.</param>
		/// <param name="futureTime">Czy czas jest w przyszłości.</param>
		/// <param name="hasProduct">Czy ma ProductId.</param>
		/// <param name="hasRecipe">Czy ma RecipeId.</param>
		[Theory]
		[InlineData(null, 100f, false, true, false)]     // Brak UserId
		[InlineData("", 100f, false, true, false)]       // Pusty UserId
		[InlineData("user", 0f, false, true, false)]     // Quantity = 0
		[InlineData("user", 15000f, false, true, false)] // Quantity > 10000
		[InlineData("user", 100f, true, true, false)]    // Czas w przyszłości
		[InlineData("user", 100f, false, false, false)]  // Brak Product i Recipe
		[InlineData("user", 100f, false, true, true)]    // Product I Recipe 
		public void ValidateMealLogEntry_InvalidEntries_ShouldReturnFalse(
			string? userId, float quantity, bool futureTime, bool hasProduct, bool hasRecipe)
		{
			// Arrange - niepoprawny wpis
			var entry = new MealLogEntry
			{
				UserId = userId!,
				ProductId = hasProduct ? Guid.NewGuid() : null,
				RecipeId = hasRecipe ? Guid.NewGuid() : null,
				Quantity = quantity,
				ConsumedAt = futureTime ? DateTime.UtcNow.AddHours(2) : DateTime.UtcNow.AddMinutes(-30),
				MealType = MealType.Breakfast
			};

			// Act & Assert
			Assert.False(_service.ValidateMealLogEntry(entry));
		}

		#endregion

		#region Helper Methods

		/// <summary>
		/// Factory method tworząca produkt testowy z określonymi wartościami odżywczymi.
		/// </summary>
		private Product CreateProduct(string name, ProductUnit unit, float calories,
			float protein, float fat, float carbs, float servingSize = 100f)
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

		/// <summary>
		/// Factory method tworząca przepis do testów.
		/// </summary>
		private Recipe CreateRecipe(string name)
		{
			return new Recipe
			{
				Id = Guid.NewGuid(),
				Name = name,
				Ingredients = new List<RecipeIngredient>()
			};
		}

		/// <summary>
		/// Factory method tworząca wpis posiłku do testów.
		/// </summary>
		private MealLogEntry CreateMealLogEntry(Guid? productId = null, Guid? recipeId = null, float quantity = 100f)
		{
			return new MealLogEntry
			{
				Id = Guid.NewGuid(),
				UserId = "test-user",
				ProductId = productId,
				RecipeId = recipeId,
				Quantity = quantity,
				MealType = MealType.Breakfast,
				ConsumedAt = DateTime.UtcNow
			};
		}

		/// <summary>
		/// Factory method tworząca wpis posiłku z już obliczonymi wartościami odżywczymi.
		/// </summary>
		private MealLogEntry CreateCalculatedMealEntry(float calories, float protein, float fat, float carbs)
		{
			return new MealLogEntry
			{
				Id = Guid.NewGuid(),
				UserId = "test-user",
				CalculatedCalories = calories,
				CalculatedProtein = protein,
				CalculatedFat = fat,
				CalculatedCarbohydrates = carbs,
				MealType = MealType.Breakfast,
				ConsumedAt = DateTime.UtcNow
			};
		}

		/// <summary>
		/// Factory method tworząca wpis posiłku z produktem.
		/// </summary>
		private MealLogEntry CreateMealWithTime(string productName, DateTime consumedAt, float calories, MealType mealType)
		{
			var entry = new MealLogEntry
			{
				Id = Guid.NewGuid(),
				UserId = "test-user",
				CalculatedCalories = calories,
				ConsumedAt = consumedAt,
				MealType = mealType,
				ProductId = Guid.NewGuid()
			};

			entry.Product = new Product
			{
				Name = productName,
				Id = entry.ProductId.Value
			};

			return entry;
		}

		#endregion
	}
}