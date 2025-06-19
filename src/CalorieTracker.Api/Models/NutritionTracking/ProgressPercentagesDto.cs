// Plik ProgressPercentagesDto.cs - DTO procentów postępu żywieniowego.

namespace CalorieTracker.Api.Models.NutritionTracking
{
	/// <summary>
	/// Model transferu danych reprezentujący procentowy postęp względem celów.
	/// </summary>
	public class ProgressPercentagesDto
	{
		/// <summary>
		/// Procent realizacji celu kalorycznego.
		/// </summary>
		public float Calories { get; set; }

		/// <summary>
		/// Procent realizacji celu białkowego.
		/// </summary>
		public float Protein { get; set; }

		/// <summary>
		/// Procent realizacji celu tłuszczowego.
		/// </summary>
		public float Fat { get; set; }

		/// <summary>
		/// Procent realizacji celu węglowodanowego.
		/// </summary>
		public float Carbohydrates { get; set; }

		/// <summary>
		/// Procent realizacji celu wodnego.
		/// </summary>
		public float Water { get; set; }
	}
}