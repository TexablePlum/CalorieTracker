// Plik RemainingNutrition.cs - Value Object reprezentujący pozostałe potrzeby żywieniowe.

namespace CalorieTracker.Domain.ValueObjects
{
	/// <summary>
	/// Value Object reprezentujący pozostałe do spożycia wartości odżywcze dla osiągnięcia dziennych celów.
	/// Kalkulowany jako różnica między celami a już spożytymi wartościami.
	/// </summary>
	public class RemainingNutrition
	{
		/// <summary>
		/// Pozostałe kalorie do spożycia.
		/// </summary>
		public float Calories { get; }

		/// <summary>
		/// Minimalne pozostałe białko do spożycia w gramach.
		/// </summary>
		public float ProteinMin { get; }

		/// <summary>
		/// Maksymalne pozostałe białko do spożycia w gramach.
		/// </summary>
		public float ProteinMax { get; }

		/// <summary>
		/// Minimalne pozostałe tłuszcze do spożycia w gramach.
		/// </summary>
		public float FatMin { get; }

		/// <summary>
		/// Maksymalne pozostałe tłuszcze do spożycia w gramach.
		/// </summary>
		public float FatMax { get; }

		/// <summary>
		/// Minimalne pozostałe węglowodany do spożycia w gramach.
		/// </summary>
		public float CarbohydratesMin { get; }

		/// <summary>
		/// Maksymalne pozostałe węglowodany do spożycia w gramach.
		/// </summary>
		public float CarbohydratesMax { get; }

		/// <summary>
		/// Pozostała woda do wypicia w litrach.
		/// </summary>
		public float Water { get; }

		/// <summary>
		/// Tworzy nową instancję pozostałych potrzeb żywieniowych.
		/// </summary>
		public RemainingNutrition(
			float calories,
			float proteinMin,
			float proteinMax,
			float fatMin,
			float fatMax,
			float carbohydratesMin,
			float carbohydratesMax,
			float water)
		{
			Calories = Math.Max(0, calories);
			ProteinMin = Math.Max(0, proteinMin);
			ProteinMax = Math.Max(0, proteinMax);
			FatMin = Math.Max(0, fatMin);
			FatMax = Math.Max(0, fatMax);
			CarbohydratesMin = Math.Max(0, carbohydratesMin);
			CarbohydratesMax = Math.Max(0, carbohydratesMax);
			Water = Math.Max(0, water);
		}
	}
}