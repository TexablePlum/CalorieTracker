// Plik ConsumedNutrition.cs - Value Object reprezentujący spożyte wartości odżywcze.

namespace CalorieTracker.Domain.ValueObjects
{
	/// <summary>
	/// Value Object reprezentujący rzeczywiste spożyte wartości odżywcze w danym dniu.
	/// Agreguje wszystkie zalogowane posiłki do sumarycznych wartości.
	/// </summary>
	public class ConsumedNutrition
	{
		/// <summary>
		/// Łączna liczba spożytych kalorii.
		/// </summary>
		public float Calories { get; }

		/// <summary>
		/// Łączna ilość spożytego białka w gramach.
		/// </summary>
		public float Protein { get; }

		/// <summary>
		/// Łączna ilość spożytego tłuszczu w gramach.
		/// </summary>
		public float Fat { get; }

		/// <summary>
		/// Łączna ilość spożytych węglowodanów w gramach.
		/// </summary>
		public float Carbohydrates { get; }

		/// <summary>
		/// Łączna ilość spożytego błonnika w gramach (opcjonalnie).
		/// </summary>
		public float? Fiber { get; }

		/// <summary>
		/// Łączna ilość spożytego cukru w gramach (opcjonalnie).
		/// </summary>
		public float? Sugar { get; }

		/// <summary>
		/// Łączna ilość spożytego sodu w miligramach (opcjonalnie).
		/// </summary>
		public float? Sodium { get; }

		/// <summary>
		/// Łączna ilość wypitej wody w litrach.
		/// </summary>
		public float WaterLiters { get; }

		/// <summary>
		/// Tworzy nową instancję spożytych wartości odżywczych.
		/// </summary>
		public ConsumedNutrition(
			float calories,
			float protein,
			float fat,
			float carbohydrates,
			float waterLiters,
			float? fiber = null,
			float? sugar = null,
			float? sodium = null)
		{
			Calories = calories;
			Protein = protein;
			Fat = fat;
			Carbohydrates = carbohydrates;
			WaterLiters = waterLiters;
			Fiber = fiber;
			Sugar = sugar;
			Sodium = sodium;
		}

		/// <summary>
		/// Tworzy pusty obiekt spożytych wartości (dla dni bez zalogowanych posiłków).
		/// </summary>
		public static ConsumedNutrition Empty => new(0, 0, 0, 0, 0);
	}
}