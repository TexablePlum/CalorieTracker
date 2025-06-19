// Plik MacronutrientRangeDto.cs - model transferu danych dla przedziału makroskładnika.
// Reprezentuje minimalne i maksymalne dzienne zapotrzebowanie na dany makroskładnik.

namespace CalorieTracker.Api.Models.Nutrition
{
	/// <summary>
	/// Model transferu danych reprezentujący przedział zapotrzebowania na makroskładnik.
	/// Encapsulates min-max range with calculated average for UI convenience.
	/// </summary>
	public class MacronutrientRangeDto
	{
		/// <summary>
		/// Minimalne dzienne zapotrzebowanie na makroskładnik w gramach.
		/// Reprezentuje dolną granicę zdrowego spożycia.
		/// </summary>
		public float MinGrams { get; set; }

		/// <summary>
		/// Maksymalne dzienne zapotrzebowanie na makroskładnik w gramach.
		/// Reprezentuje górną granicę optymalnego spożycia.
		/// </summary>
		public float MaxGrams { get; set; }

		/// <summary>
		/// Średnia wartość zapotrzebowania na makroskładnik w gramach.
		/// Obliczona jako (Min + Max) / 2 dla wygody UI.
		/// </summary>
		public float AverageGrams => (MinGrams + MaxGrams) / 2f;

		/// <summary>
		/// Minimalne zapotrzebowanie wyrażone jako procent dziennych kalorii.
		/// Przydatne dla analizy dystrybucji makroskładników.
		/// </summary>
		public float MinPercentageOfCalories { get; set; }

		/// <summary>
		/// Maksymalne zapotrzebowanie wyrażone jako procent dziennych kalorii.
		/// Przydatne dla analizy dystrybucji makroskładników.
		/// </summary>
		public float MaxPercentageOfCalories { get; set; }

		/// <summary>
		/// Średni procent dziennych kalorii z tego makroskładnika.
		/// Obliczony jako (MinPercentage + MaxPercentage) / 2.
		/// </summary>
		public float AveragePercentageOfCalories => (MinPercentageOfCalories + MaxPercentageOfCalories) / 2f;
	}
}