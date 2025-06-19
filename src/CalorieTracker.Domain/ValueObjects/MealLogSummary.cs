// Plik MealLogSummary.cs - Value Object podsumowania posiłku dla daily progress.

using CalorieTracker.Domain.Enums;

namespace CalorieTracker.Domain.ValueObjects
{
	/// <summary>
	/// Value Object reprezentujący uproszczone informacje o posiłku dla daily progress.
	/// Zawiera tylko najważniejsze dane potrzebne do wyświetlenia listy posiłków dnia.
	/// </summary>
	public class MealLogSummary
	{
		/// <summary>
		/// Identyfikator wpisu posiłku.
		/// </summary>
		public Guid Id { get; }

		/// <summary>
		/// Typ posiłku (śniadanie, obiad, etc.).
		/// </summary>
		public MealType MealType { get; }

		/// <summary>
		/// Nazwa produktu lub przepisu.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Spożyta ilość z jednostką.
		/// </summary>
		public string QuantityWithUnit { get; }

		/// <summary>
		/// Kalorie spożytego posiłku.
		/// </summary>
		public float Calories { get; }

		/// <summary>
		/// Czas spożycia w formacie HH:mm.
		/// </summary>
		public string ConsumedTime { get; }

		/// <summary>
		/// Tworzy nową instancję podsumowania posiłku.
		/// </summary>
		public MealLogSummary(
			Guid id,
			MealType mealType,
			string name,
			string quantityWithUnit,
			float calories,
			string consumedTime)
		{
			Id = id;
			MealType = mealType;
			Name = name;
			QuantityWithUnit = quantityWithUnit;
			Calories = calories;
			ConsumedTime = consumedTime;
		}
	}
}