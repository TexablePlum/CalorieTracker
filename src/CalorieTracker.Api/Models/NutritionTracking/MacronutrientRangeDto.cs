// Plik MacronutrientRangeDto.cs - DTO przedziału makroskładnika.

namespace CalorieTracker.Api.Models.NutritionTracking
{
	/// <summary>
	/// Model transferu danych reprezentujący przedział zapotrzebowania na makroskładnik.
	/// </summary>
	public class MacronutrientRangeDto
	{
		/// <summary>
		/// Minimalne dzienne zapotrzebowanie w gramach.
		/// </summary>
		public float MinGrams { get; set; }

		/// <summary>
		/// Maksymalne dzienne zapotrzebowanie w gramach.
		/// </summary>
		public float MaxGrams { get; set; }

		/// <summary>
		/// Średnia wartość zapotrzebowania w gramach.
		/// </summary>
		public float AverageGrams => (MinGrams + MaxGrams) / 2f;
	}
}