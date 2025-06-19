// Plik DailyNutritionProgress.cs - Value Object reprezentujący dzienny postęp żywieniowy.
// Łączy cele użytkownika z rzeczywistym spożyciem i kalkuluje pozostałe potrzeby.

using CalorieTracker.Domain.ValueObjects;

namespace CalorieTracker.Domain.ValueObjects
{
	/// <summary>
	/// Value Object reprezentujący kompletny dzienny postęp żywieniowy użytkownika.
	/// Łączy dzienne cele z rzeczywistym spożyciem i kalkuluje pozostałe potrzeby oraz procenty postępu.
	/// Immutable object zapewniający spójność danych i enkapsulację logiki biznesowej.
	/// </summary>
	public class DailyNutritionProgress
	{
		/// <summary>
		/// Data, której dotyczy ten postęp żywieniowy.
		/// </summary>
		public DateOnly Date { get; }

		/// <summary>
		/// Dzienne cele żywieniowe użytkownika na ten dzień.
		/// </summary>
		public DailyNutritionRequirements Goals { get; }

		/// <summary>
		/// Rzeczywiste spożyte wartości odżywcze w tym dniu.
		/// </summary>
		public ConsumedNutrition Consumed { get; }

		/// <summary>
		/// Pozostałe do spożycia wartości dla osiągnięcia celów.
		/// </summary>
		public RemainingNutrition Remaining { get; }

		/// <summary>
		/// Procentowy postęp względem celów dziennych.
		/// </summary>
		public NutritionProgressPercentages ProgressPercentages { get; }

		/// <summary>
		/// Lista wszystkich posiłków zalogowanych w tym dniu.
		/// </summary>
		public IReadOnlyList<MealLogSummary> MealsToday { get; }

		/// <summary>
		/// Tworzy nową instancję dziennego postępu żywieniowego.
		/// </summary>
		/// <param name="date">Data postępu.</param>
		/// <param name="goals">Cele żywieniowe.</param>
		/// <param name="consumed">Spożyte wartości.</param>
		/// <param name="mealsToday">Lista posiłków dnia.</param>
		public DailyNutritionProgress(
			DateOnly date,
			DailyNutritionRequirements goals,
			ConsumedNutrition consumed,
			IReadOnlyList<MealLogSummary> mealsToday)
		{
			Date = date;
			Goals = goals;
			Consumed = consumed;
			MealsToday = mealsToday;

			// Kalkuluje pozostałe wartości i procenty
			Remaining = CalculateRemaining(goals, consumed);
			ProgressPercentages = CalculateProgressPercentages(goals, consumed);
		}

		/// <summary>
		/// Oblicza pozostałe do spożycia wartości odżywcze.
		/// </summary>
		private static RemainingNutrition CalculateRemaining(
			DailyNutritionRequirements goals,
			ConsumedNutrition consumed)
		{
			return new RemainingNutrition(
				calories: Math.Max(0, goals.Calories - consumed.Calories),
				proteinMin: Math.Max(0, goals.ProteinMinGrams - consumed.Protein),
				proteinMax: Math.Max(0, goals.ProteinMaxGrams - consumed.Protein),
				fatMin: Math.Max(0, goals.FatMinGrams - consumed.Fat),
				fatMax: Math.Max(0, goals.FatMaxGrams - consumed.Fat),
				carbohydratesMin: Math.Max(0, goals.CarbohydratesMinGrams - consumed.Carbohydrates),
				carbohydratesMax: Math.Max(0, goals.CarbohydratesMaxGrams - consumed.Carbohydrates),
				water: Math.Max(0, goals.WaterLiters - consumed.WaterLiters)
			);
		}

		/// <summary>
		/// Oblicza procentowy postęp względem celów.
		/// </summary>
		private static NutritionProgressPercentages CalculateProgressPercentages(
			DailyNutritionRequirements goals,
			ConsumedNutrition consumed)
		{
			return new NutritionProgressPercentages(
				calories: CalculatePercentage(consumed.Calories, goals.Calories),
				protein: CalculatePercentage(consumed.Protein, goals.ProteinAverageGrams),
				fat: CalculatePercentage(consumed.Fat, goals.FatAverageGrams),
				carbohydrates: CalculatePercentage(consumed.Carbohydrates, goals.CarbohydratesAverageGrams),
				water: CalculatePercentage(consumed.WaterLiters, goals.WaterLiters)
			);
		}

		/// <summary>
		/// Metoda pomocnicza do kalkulacji procentu z zabezpieczeniem przed dzieleniem przez zero.
		/// </summary>
		private static float CalculatePercentage(float consumed, float goal)
		{
			if (goal <= 0) return 0f;
			var percentage = (consumed / goal) * 100f;
			return (float)Math.Round(Math.Min(percentage, 999f), 1); // Max 999% dla bezpieczeństwa UI
		}

		/// <summary>
		/// Sprawdza czy użytkownik osiągnął swoje dzienne cele kaloryczne.
		/// </summary>
		public bool HasMetCalorieGoal => Consumed.Calories >= Goals.Calories;

		/// <summary>
		/// Sprawdza czy użytkownik jest w przedziale celu białkowego.
		/// </summary>
		public bool IsInProteinRange =>
			Consumed.Protein >= Goals.ProteinMinGrams &&
			Consumed.Protein <= Goals.ProteinMaxGrams;

		/// <summary>
		/// Sprawdza czy wszystkie makroskładniki są w optymalnych przedziałach.
		/// </summary>
		public bool AreAllMacrosOptimal =>
			IsInProteinRange &&
			Consumed.Fat >= Goals.FatMinGrams && Consumed.Fat <= Goals.FatMaxGrams &&
			Consumed.Carbohydrates >= Goals.CarbohydratesMinGrams &&
			Consumed.Carbohydrates <= Goals.CarbohydratesMaxGrams;
	}
}