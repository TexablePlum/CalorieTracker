// Plik QuickWaterIntakeDto.cs - model transferu danych dla szybkich opcji dodawania wody.

namespace CalorieTracker.Api.Models.NutritionTracking
{
	/// <summary>
	/// Model transferu danych dla szybkich opcji dodawania wody.
	/// Używany do wyświetlania predefiniowanych ilości (szklanka, butelka, etc.).
	/// </summary>
	public class QuickWaterIntakeDto
	{
		/// <summary>
		/// Nazwa opcji (np. "Szklanka", "Butelka", "Duża butelka").
		/// </summary>
		public string Name { get; set; } = null!;

		/// <summary>
		/// Ilość w mililitrach.
		/// </summary>
		public float AmountMilliliters { get; set; }

		/// <summary>
		/// Opis opcji.
		/// </summary>
		public string Description { get; set; } = null!;

		/// <summary>
		/// Ilość w litrach (obliczana automatycznie).
		/// </summary>
		public float AmountLiters => AmountMilliliters / 1000f;
	}
}