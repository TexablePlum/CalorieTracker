// Plik ProductSummaryDto.cs - uproszczony model produktu dla list i wyszukiwania.
// Odpowiada za reprezentację podstawowych informacji o produkcie w wynikach wyszukiwania i listach.

namespace CalorieTracker.Api.Models.Products
{
	/// <summary>
	/// Uproszczony model DTO produktu używany w listach i wynikach wyszukiwania.
	/// Zawiera tylko kluczowe informacje potrzebne do prezentacji produktu w interfejsie użytkownika.
	/// </summary>
	public class ProductSummaryDto
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
		/// Flaga wskazująca czy produkt został zweryfikowany przez administratora.
		/// </summary>
		public bool IsVerified { get; set; }
	}
}