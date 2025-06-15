// Plik ProductDto.cs - model danych produktu zwracany przez API.
// Odpowiada za reprezentację pełnych informacji o produkcie w odpowiedziach API.

namespace CalorieTracker.Api.Models.Products
{
	/// <summary>
	/// Model DTO reprezentujący pełne dane produktu w systemie.
	/// Zawiera podstawowe informacje, dane żywieniowe oraz metadane dotyczące produktu.
	/// </summary>
	public class ProductDto
	{
		/// <summary>
		/// Unikalny identyfikator produktu.
		/// </summary>
		public Guid Id { get; set; }

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
		/// Wielkość porcji w podanych jednostkach.
		/// </summary>
		public float ServingSize { get; set; }

		/// <summary>
		/// Kalorie na 100g/ml produktu.
		/// </summary>
		public float CaloriesPer100g { get; set; }

		/// <summary>
		/// Białko na 100g/ml produktu.
		/// </summary>
		public float ProteinPer100g { get; set; }

		/// <summary>
		/// Tłuszcze na 100g/ml produktu.
		/// </summary>
		public float FatPer100g { get; set; }

		/// <summary>
		/// Węglowodany na 100g/ml produktu.
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

		/// <summary>
		/// Flaga wskazująca czy produkt został zweryfikowany przez administratora.
		/// </summary>
		public bool IsVerified { get; set; }

		/// <summary>
		/// Identyfikator użytkownika, który dodał produkt (opcjonalne).
		/// </summary>
		public string? CreatedByUserId { get; set; }

		/// <summary>
		/// Data i czas utworzenia produktu.
		/// </summary>
		public DateTime CreatedAt { get; set; }

		/// <summary>
		/// Data i czas ostatniej aktualizacji produktu.
		/// </summary>
		public DateTime UpdatedAt { get; set; }
	}
}