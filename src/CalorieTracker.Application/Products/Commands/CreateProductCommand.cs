// Plik CreateProductCommand.cs - definicja komendy tworzenia produktu.
// Przenosi dane wymagane do utworzenia nowego produktu w systemie.

using CalorieTracker.Domain.Enums;

namespace CalorieTracker.Application.Products.Commands
{
	/// <summary>
	/// Rekord reprezentujący komendę tworzenia nowego produktu spożywczego.
	/// Zawiera pełne dane żywieniowe oraz metadane produktu.
	/// </summary>
	public record CreateProductCommand
	{
		/// <summary>
		/// Nazwa produktu. Pole wymagane.
		/// </summary>
		public string Name { get; init; } = null!;

		/// <summary>
		/// Marka/producent produktu. Pole opcjonalne.
		/// </summary>
		public string? Brand { get; init; }

		/// <summary>
		/// Opis produktu. Pole opcjonalne.
		/// </summary>
		public string? Description { get; init; }

		/// <summary>
		/// Składniki produktu. Pole opcjonalne.
		/// </summary>
		public string? Ingredients { get; init; }

		/// <summary>
		/// Kod kreskowy produktu (EAN/UPC). Pole opcjonalne.
		/// </summary>
		public string? Barcode { get; init; }

		/// <summary>
		/// Kategoria produktu zdefiniowana w enumie ProductCategory.
		/// </summary>
		public ProductCategory Category { get; init; }

		/// <summary>
		/// Jednostka miary produktu zdefiniowana w enumie ProductUnit.
		/// </summary>
		public ProductUnit Unit { get; init; }

		/// <summary>
		/// Wielkość porcji produktu w podanych jednostkach.
		/// </summary>
		public float ServingSize { get; init; }

		/// <summary>
		/// Ilość kalorii na 100g/ml produktu.
		/// </summary>
		public float CaloriesPer100g { get; init; }

		/// <summary>
		/// Ilość białka na 100g/ml produktu.
		/// </summary>
		public float ProteinPer100g { get; init; }

		/// <summary>
		/// Ilość tłuszczu na 100g/ml produktu.
		/// </summary>
		public float FatPer100g { get; init; }

		/// <summary>
		/// Ilość węglowodanów na 100g/ml produktu.
		/// </summary>
		public float CarbohydratesPer100g { get; init; }

		/// <summary>
		/// Ilość błonnika na 100g/ml produktu. Pole opcjonalne.
		/// </summary>
		public float? FiberPer100g { get; init; }

		/// <summary>
		/// Ilość cukrów na 100g/ml produktu. Pole opcjonalne.
		/// </summary>
		public float? SugarsPer100g { get; init; }

		/// <summary>
		/// Ilość sodu na 100g/ml produktu. Pole opcjonalne.
		/// </summary>
		public float? SodiumPer100g { get; init; }

		/// <summary>
		/// Identyfikator użytkownika tworzącego produkt. Pole wymagane.
		/// </summary>
		public string CreatedByUserId { get; init; } = null!;
	}
}