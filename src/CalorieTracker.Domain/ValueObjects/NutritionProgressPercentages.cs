// Plik NutritionProgressPercentages.cs - Value Object z procentami postępu żywieniowego.

namespace CalorieTracker.Domain.ValueObjects
{
	/// <summary>
	/// Value Object reprezentujący procentowy postęp względem dziennych celów żywieniowych.
	/// Używany do wizualizacji progress barów i gamification.
	/// </summary>
	public class NutritionProgressPercentages
	{
		/// <summary>
		/// Procent realizacji celu kalorycznego (0-999%).
		/// </summary>
		public float Calories { get; }

		/// <summary>
		/// Procent realizacji celu białkowego (0-999%).
		/// </summary>
		public float Protein { get; }

		/// <summary>
		/// Procent realizacji celu tłuszczowego (0-999%).
		/// </summary>
		public float Fat { get; }

		/// <summary>
		/// Procent realizacji celu węglowodanowego (0-999%).
		/// </summary>
		public float Carbohydrates { get; }

		/// <summary>
		/// Procent realizacji celu wodnego (0-999%).
		/// </summary>
		public float Water { get; }

		/// <summary>
		/// Tworzy nową instancję procentów postępu żywieniowego.
		/// </summary>
		public NutritionProgressPercentages(
			float calories,
			float protein,
			float fat,
			float carbohydrates,
			float water)
		{
			Calories = calories;
			Protein = protein;
			Fat = fat;
			Carbohydrates = carbohydrates;
			Water = water;
		}

		/// <summary>
		/// Średni procent postępu wszystkich makroskładników.
		/// </summary>
		public float AverageMacroProgress => (Protein + Fat + Carbohydrates) / 3f;

		/// <summary>
		/// Sprawdza czy użytkownik przekroczył którykolwiek z celów o więcej niż 20%.
		/// </summary>
		public bool HasSignificantOverage =>
			Calories > 120f || Protein > 120f || Fat > 120f || Carbohydrates > 120f;
	}
}