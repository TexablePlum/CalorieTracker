// Plik UpdateProductRequest.cs - model żądania aktualizacji produktu.
// Odpowiada za przechwytywanie danych wymaganych do aktualizacji istniejącego produktu w systemie.

namespace CalorieTracker.Api.Models.Products
{
	/// <summary>
	/// Model żądania aktualizacji danych produktu.
	/// Zawiera wszystkie możliwe do zaktualizowania pola produktu wraz z danymi żywieniowymi.
	/// </summary>
	public class UpdateProductRequest
	{
		/// <summary>
		/// Zaktualizowana nazwa produktu. Pole wymagane.
		/// </summary>
		public string Name { get; set; } = null!;

		/// <summary>
		/// Zaktualizowana marka/producent produktu (opcjonalne).
		/// </summary>
		public string? Brand { get; set; }

		/// <summary>
		/// Zaktualizowany opis produktu (opcjonalne).
		/// </summary>
		public string? Description { get; set; }

		/// <summary>
		/// Zaktualizowane składniki produktu (opcjonalne).
		/// </summary>
		public string? Ingredients { get; set; }

		/// <summary>
		/// Zaktualizowany kod kreskowy produktu (opcjonalne).
		/// </summary>
		public string? Barcode { get; set; }

		/// <summary>
		/// Zaktualizowana kategoria produktu. Pole wymagane.
		/// </summary>
		public string Category { get; set; } = null!;

		/// <summary>
		/// Zaktualizowana jednostka miary produktu (np. 'g', 'ml'). Pole wymagane.
		/// </summary>
		public string Unit { get; set; } = null!;

		/// <summary>
		/// Zaktualizowana wielkość porcji w podanych jednostkach.
		/// </summary>
		public float ServingSize { get; set; }

		/// <summary>
		/// Zaktualizowane kalorie na 100g/ml produktu.
		/// </summary>
		public float CaloriesPer100g { get; set; }

		/// <summary>
		/// Zaktualizowane białko na 100g/ml produktu.
		/// </summary>
		public float ProteinPer100g { get; set; }

		/// <summary>
		/// Zaktualizowane tłuszcze na 100g/ml produktu.
		/// </summary>
		public float FatPer100g { get; set; }

		/// <summary>
		/// Zaktualizowane węglowodany na 100g/ml produktu.
		/// </summary>
		public float CarbohydratesPer100g { get; set; }

		/// <summary>
		/// Zaktualizowany błonnik na 100g/ml produktu (opcjonalne).
		/// </summary>
		public float? FiberPer100g { get; set; }

		/// <summary>
		/// Zaktualizowane cukry na 100g/ml produktu (opcjonalne).
		/// </summary>
		public float? SugarsPer100g { get; set; }

		/// <summary>
		/// Zaktualizowany sód na 100g/ml produktu (opcjonalne).
		/// </summary>
		public float? SodiumPer100g { get; set; }
	}
}