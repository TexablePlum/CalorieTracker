// Plik CreateProductRequest.cs - model żądania tworzenia nowego produktu.
// Odpowiada za przechwytywanie danych wymaganych do dodania nowego produktu do systemu.

namespace CalorieTracker.Api.Models.Products
{
	/// <summary>
	/// Model żądania tworzenia nowego produktu w systemie.
	/// Zawiera pełne dane żywieniowe oraz informacje opisowe produktu.
	/// </summary>
	public class CreateProductRequest
	{
		/// <summary>
		/// Nazwa produktu. Pole wymagane.
		/// </summary>
		public string Name { get; set; } = null!;

		/// <summary>
		/// Marka/producent produktu (opcjonalne).
		/// </summary>
		public string? Brand { get; set; }

		/// <summary>
		/// Opis produktu (opcjonalne).
		/// </summary>
		public string? Description { get; set; }

		/// <summary>
		/// Składniki produktu (opcjonalne).
		/// </summary>
		public string? Ingredients { get; set; }

		/// <summary>
		/// Kod kreskowy produktu (opcjonalne).
		/// </summary>
		public string? Barcode { get; set; }

		/// <summary>
		/// Kategoria produktu. Pole wymagane.
		/// </summary>
		public string Category { get; set; } = null!;

		/// <summary>
		/// Jednostka miary produktu (np. 'g', 'ml'). Pole wymagane.
		/// </summary>
		public string Unit { get; set; } = null!;

		/// <summary>
		/// Wielkość porcji w podanych jednostkach. Pole wymagane.
		/// </summary>
		public float ServingSize { get; set; }

		/// <summary>
		/// Kalorie na 100g/ml produktu. Pole wymagane.
		/// </summary>
		public float CaloriesPer100g { get; set; }

		/// <summary>
		/// Białko na 100g/ml produktu. Pole wymagane.
		/// </summary>
		public float ProteinPer100g { get; set; }

		/// <summary>
		/// Tłuszcze na 100g/ml produktu. Pole wymagane.
		/// </summary>
		public float FatPer100g { get; set; }

		/// <summary>
		/// Węglowodany na 100g/ml produktu. Pole wymagane.
		/// </summary>
		public float CarbohydratesPer100g { get; set; }

		/// <summary>
		/// Błonnik na 100g/ml produktu (opcjonalne).
		/// </summary>
		public float? FiberPer100g { get; set; }

		/// <summary>
		/// Cukry na 100g/ml produktu (opcjonalne).
		/// </summary>
		public float? SugarsPer100g { get; set; }

		/// <summary>
		/// Sód na 100g/ml produktu (opcjonalne).
		/// </summary>
		public float? SodiumPer100g { get; set; }
	}
}