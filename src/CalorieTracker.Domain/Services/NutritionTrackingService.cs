// Plik NutritionTrackingService.cs - serwis domenowy do zarządzania śledzeniem posiłków.
// Odpowiada za kalkulację wartości odżywczych posiłków i agregację dziennego spożycia.

using CalorieTracker.Domain.Entities;
using CalorieTracker.Domain.Enums;
using CalorieTracker.Domain.Services.Interfaces;
using CalorieTracker.Domain.ValueObjects;

namespace CalorieTracker.Domain.Services
{
	/// <summary>
	/// Serwis domenowy do zarządzania śledzeniem posiłków i kalkulacji wartości odżywczych.
	/// Zawiera logikę biznesową do przeliczania nutrition dla pojedynczych posiłków oraz agregacji dziennego spożycia.
	/// Współpracuje z istniejącymi serwisami kalkulacji żywieniowych.
	/// </summary>
	public class NutritionTrackingService
	{
		/// <summary>
		/// Serwis kalkulacji wartości odżywczych przepisów.
		/// </summary>
		private readonly IRecipeNutritionCalculator _recipeNutritionCalculator;

		/// <summary>
		/// Serwis kalkulacji dziennych wymagań żywieniowych.
		/// </summary>
		private readonly NutritionCalculationService _nutritionCalculationService;

		/// <summary>
		/// Inicjalizuje nową instancję serwisu śledzenia żywienia.
		/// </summary>
		/// <param name="recipeNutritionCalculator">Serwis kalkulacji przepisów.</param>
		/// <param name="nutritionCalculationService">Serwis kalkulacji wymagań żywieniowych.</param>
		public NutritionTrackingService(
			IRecipeNutritionCalculator recipeNutritionCalculator,
			NutritionCalculationService nutritionCalculationService)
		{
			_recipeNutritionCalculator = recipeNutritionCalculator;
			_nutritionCalculationService = nutritionCalculationService;
		}

		/// <summary>
		/// Kalkuluje i wypełnia wartości odżywcze dla wpisu posiłku na podstawie produktu lub przepisu.
		/// Automatycznie przelicza wartości na rzeczywistą spożytą ilość.
		/// </summary>
		/// <param name="mealLogEntry">Wpis posiłku do uzupełnienia wartościami odżywczymi.</param>
		/// <exception cref="ArgumentException">Gdy wpis nie ma powiązanego produktu ani przepisu.</exception>
		public void CalculateNutritionForMealEntry(MealLogEntry mealLogEntry)
		{
			if (mealLogEntry.IsProductBased && mealLogEntry.Product != null)
			{
				CalculateNutritionFromProduct(mealLogEntry, mealLogEntry.Product);
			}
			else if (mealLogEntry.IsRecipeBased && mealLogEntry.Recipe != null)
			{
				CalculateNutritionFromRecipe(mealLogEntry, mealLogEntry.Recipe);
			}
			else
			{
				throw new ArgumentException("MealLogEntry musi mieć powiązany Product lub Recipe");
			}

			mealLogEntry.UpdatedAt = DateTime.UtcNow;
		}

		/// <summary>
		/// Kalkuluje wartości odżywcze na podstawie produktu spożywczego.
		/// Przelicza wartości z "na 100g" na rzeczywistą spożytą ilość.
		/// </summary>
		/// <param name="mealLogEntry">Wpis posiłku do uzupełnienia.</param>
		/// <param name="product">Produkt spożywczy będący podstawą kalkulacji.</param>
		private void CalculateNutritionFromProduct(MealLogEntry mealLogEntry, Product product)
		{
			// Konwertuje ilość na gramy
			var quantityInGrams = ConvertToGrams(mealLogEntry.Quantity, product);

			// Współczynnik przeliczeniowy z "na 100g" na rzeczywistą ilość
			var factor = quantityInGrams / 100f;

			// Kalkuluje podstawowe makroskładniki
			mealLogEntry.CalculatedCalories = product.CaloriesPer100g * factor;
			mealLogEntry.CalculatedProtein = product.ProteinPer100g * factor;
			mealLogEntry.CalculatedFat = product.FatPer100g * factor;
			mealLogEntry.CalculatedCarbohydrates = product.CarbohydratesPer100g * factor;

			// Kalkuluje opcjonalne składniki (jeśli dostępne)
			mealLogEntry.CalculatedFiber = product.FiberPer100g.HasValue ?
				product.FiberPer100g.Value * factor : null;
			mealLogEntry.CalculatedSugar = product.SugarsPer100g.HasValue ?
				product.SugarsPer100g.Value * factor : null;
			mealLogEntry.CalculatedSodium = product.SodiumPer100g.HasValue ?
				product.SodiumPer100g.Value * factor : null;

			// Zaokrągla wyniki do 1 miejsca po przecinku
			RoundNutritionValues(mealLogEntry);
		}

		/// <summary>
		/// Kalkuluje wartości odżywcze na podstawie przepisu kulinarnego.
		/// Używa RecipeNutritionCalculator do obliczenia całkowitej wartości przepisu,
		/// następnie skaluje do rzeczywistej spożytej porcji.
		/// </summary>
		/// <param name="mealLogEntry">Wpis posiłku do uzupełnienia.</param>
		/// <param name="recipe">Przepis kulinarny będący podstawą kalkulacji.</param>
		private void CalculateNutritionFromRecipe(MealLogEntry mealLogEntry, Recipe recipe)
		{
			var totalRecipeNutrition = _recipeNutritionCalculator.CalculateForRecipe(recipe);
			var totalRecipeWeight = 100f;
			var factor = mealLogEntry.Quantity / totalRecipeWeight;

			mealLogEntry.CalculatedCalories = totalRecipeNutrition.Calories * factor;
			mealLogEntry.CalculatedProtein = totalRecipeNutrition.Protein * factor;
			mealLogEntry.CalculatedFat = totalRecipeNutrition.Fat * factor;
			mealLogEntry.CalculatedCarbohydrates = totalRecipeNutrition.Carbohydrates * factor;

			mealLogEntry.CalculatedFiber = totalRecipeNutrition.Fiber.HasValue ?
				totalRecipeNutrition.Fiber.Value * factor : null;
			mealLogEntry.CalculatedSugar = totalRecipeNutrition.Sugar.HasValue ?
				totalRecipeNutrition.Sugar.Value * factor : null;
			mealLogEntry.CalculatedSodium = totalRecipeNutrition.Sodium.HasValue ?
				totalRecipeNutrition.Sodium.Value * factor : null;

			RoundNutritionValues(mealLogEntry);
		}

		/// <summary>
		/// Konwertuje ilość na gramy w zależności od jednostki produktu.
		/// Używa tej samej logiki co RecipeNutritionCalculator dla spójności.
		/// </summary>
		/// <param name="quantity">Ilość w jednostkach produktu.</param>
		/// <param name="product">Produkt spożywczy z informacją o jednostce.</param>
		/// <returns>Ilość w gramach.</returns>
		private static float ConvertToGrams(float quantity, Product product)
		{
			return product.Unit switch
			{
				ProductUnit.Piece => quantity * product.ServingSize,
				ProductUnit.Gram => quantity,                        
				ProductUnit.Milliliter => quantity,                  // Założenie: 1ml ≈ 1g
				_ => throw new ArgumentException($"Nieobsługiwana jednostka: {product.Unit}")
			};
		}

		/// <summary>
		/// Zaokrągla wszystkie wartości odżywcze do 1 miejsca po przecinku dla spójności.
		/// </summary>
		/// <param name="mealLogEntry">Wpis posiłku do zaokrąglenia wartości.</param>
		private static void RoundNutritionValues(MealLogEntry mealLogEntry)
		{
			mealLogEntry.CalculatedCalories = (float)Math.Round(mealLogEntry.CalculatedCalories, 1);
			mealLogEntry.CalculatedProtein = (float)Math.Round(mealLogEntry.CalculatedProtein, 1);
			mealLogEntry.CalculatedFat = (float)Math.Round(mealLogEntry.CalculatedFat, 1);
			mealLogEntry.CalculatedCarbohydrates = (float)Math.Round(mealLogEntry.CalculatedCarbohydrates, 1);

			if (mealLogEntry.CalculatedFiber.HasValue)
				mealLogEntry.CalculatedFiber = (float)Math.Round(mealLogEntry.CalculatedFiber.Value, 1);

			if (mealLogEntry.CalculatedSugar.HasValue)
				mealLogEntry.CalculatedSugar = (float)Math.Round(mealLogEntry.CalculatedSugar.Value, 1);

			if (mealLogEntry.CalculatedSodium.HasValue)
				mealLogEntry.CalculatedSodium = (float)Math.Round(mealLogEntry.CalculatedSodium.Value, 1);
		}

		/// <summary>
		/// Agreguje wszystkie posiłki z danego dnia do obiektów ConsumedNutrition.
		/// Sumuje wartości odżywcze ze wszystkich zalogowanych posiłków.
		/// </summary>
		/// <param name="mealLogEntries">Lista wszystkich posiłków z danego dnia.</param>
		/// <param name="waterIntakeLiters">Spożyta woda w litrach (domyślnie 0).</param>
		/// <returns>Zagregowane spożyte wartości odżywcze.</returns>
		public ConsumedNutrition AggregateConsumedNutrition(
			IEnumerable<MealLogEntry> mealLogEntries,
			float waterIntakeLiters = 0f)
		{
			var totalCalories = 0f;
			var totalProtein = 0f;
			var totalFat = 0f;
			var totalCarbohydrates = 0f;
			var totalFiber = 0f;
			var totalSugar = 0f;
			var totalSodium = 0f;

			foreach (var meal in mealLogEntries)
			{
				totalCalories += meal.CalculatedCalories;
				totalProtein += meal.CalculatedProtein;
				totalFat += meal.CalculatedFat;
				totalCarbohydrates += meal.CalculatedCarbohydrates;

				if (meal.CalculatedFiber.HasValue)
					totalFiber += meal.CalculatedFiber.Value;

				if (meal.CalculatedSugar.HasValue)
					totalSugar += meal.CalculatedSugar.Value;

				if (meal.CalculatedSodium.HasValue)
					totalSodium += meal.CalculatedSodium.Value;
			}

			return new ConsumedNutrition(
				calories: (float)Math.Round(totalCalories, 1),
				protein: (float)Math.Round(totalProtein, 1),
				fat: (float)Math.Round(totalFat, 1),
				carbohydrates: (float)Math.Round(totalCarbohydrates, 1),
				waterLiters: (float)Math.Round(waterIntakeLiters, 1),
				fiber: totalFiber > 0 ? (float)Math.Round(totalFiber, 1) : null,
				sugar: totalSugar > 0 ? (float)Math.Round(totalSugar, 1) : null,
				sodium: totalSodium > 0 ? (float)Math.Round(totalSodium, 1) : null
			);
		}

		/// <summary>
		/// Tworzy listę podsumowań posiłków dla daily progress view.
		/// Konwertuje szczegółowe MealLogEntry na uproszczone MealLogSummary.
		/// </summary>
		/// <param name="mealLogEntries">Lista szczegółowych wpisów posiłków.</param>
		/// <returns>Lista podsumowań posiłków uporządkowana chronologicznie.</returns>
		public IReadOnlyList<MealLogSummary> CreateMealSummaries(IEnumerable<MealLogEntry> mealLogEntries)
		{
			return mealLogEntries
				.OrderBy(m => m.ConsumedAt)
				.Select(meal => new MealLogSummary(
					id: meal.Id,
					mealType: meal.MealType,
					name: meal.GetDisplayName(),
					quantityWithUnit: $"{meal.Quantity:F1} {meal.GetQuantityUnit()}",
					calories: meal.CalculatedCalories,
					consumedTime: meal.ConsumedAt.ToString("HH:mm")
				))
				.ToList()
				.AsReadOnly();
		}

		/// <summary>
		/// Tworzy kompletny obiekt DailyNutritionProgress łączący cele, spożycie i postęp.
		/// Główna metoda orchestrująca wszystkie kalkulacje dla daily progress view.
		/// </summary>
		/// <param name="date">Data dla której tworzony jest postęp.</param>
		/// <param name="userProfile">Profil użytkownika z celami żywieniowymi.</param>
		/// <param name="mealLogEntries">Lista posiłków z tego dnia.</param>
		/// <param name="waterIntakeLiters">Spożyta woda w litrach.</param>
		/// <returns>Kompletny obiekt z postępem żywieniowym dnia.</returns>
		public DailyNutritionProgress CreateDailyProgress(
			DateOnly date,
			UserProfile userProfile,
			IEnumerable<MealLogEntry> mealLogEntries,
			float waterIntakeLiters = 0f)
		{
			// Oblicza dzienne cele żywieniowe
			var goals = _nutritionCalculationService.CalculateDailyRequirements(userProfile);

			// Agreguje spożyte wartości
			var consumed = AggregateConsumedNutrition(mealLogEntries, waterIntakeLiters);

			// Tworzy podsumowania posiłków
			var mealSummaries = CreateMealSummaries(mealLogEntries);

			// Tworzy kompletny obiekt postępu
			return new DailyNutritionProgress(date, goals, consumed, mealSummaries);
		}

		/// <summary>
		/// Waliduje czy MealLogEntry ma wszystkie wymagane dane do kalkulacji nutrition.
		/// </summary>
		/// <param name="mealLogEntry">Wpis posiłku do walidacji.</param>
		/// <returns>True jeśli wpis jest poprawny, false w przeciwnym razie.</returns>
		public bool ValidateMealLogEntry(MealLogEntry mealLogEntry)
		{
			// Sprawdza podstawowe wymagania
			if (string.IsNullOrEmpty(mealLogEntry.UserId))
				return false;

			if (mealLogEntry.Quantity <= 0 || mealLogEntry.Quantity > 10000)
				return false;

			var hasProduct = mealLogEntry.ProductId.HasValue;
			var hasRecipe = mealLogEntry.RecipeId.HasValue;

			if (hasProduct == hasRecipe) // Oba true lub oba false - nieprawidłowe
				return false;

			// Sprawdza czy ConsumedAt nie jest w przyszłości
			if (mealLogEntry.ConsumedAt > DateTime.UtcNow.AddHours(1)) // 1h tolerancji na strefy czasowe
				return false;

			return true;
		}
	}
}