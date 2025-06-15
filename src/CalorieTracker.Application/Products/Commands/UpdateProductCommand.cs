// Plik UpdateProductCommand.cs - definicja komendy aktualizacji produktu.
// Przenosi dane wymagane do modyfikacji istniejącego produktu w systemie.

using CalorieTracker.Domain.Enums;

namespace CalorieTracker.Application.Auth.Commands
{
	/// <summary>
	/// Rekord reprezentujący komendę aktualizacji produktu spożywczego.
	/// Zawiera pełne dane żywieniowe oraz metadane produktu do zaktualizowania.
	/// </summary>
	public record UpdateProductCommand
	{
		/// <summary>
		/// Unikalny identyfikator produktu do aktualizacji.
		/// </summary>
		public Guid Id { get; init; }

		/// <summary>
		/// Zaktualizowana nazwa produktu. Pole wymagane.
		/// </summary>
		public string Name { get; init; } = null!;

		/// <summary>
		/// Zaktualizowana marka/producent produktu. Pole opcjonalne.
		/// </summary>
		public string? Brand { get; init; }

		/// <summary>
		/// Zaktualizowany opis produktu. Pole opcjonalne.
		/// </summary>
		public string? Description { get; init; }

		/// <summary>
		/// Zaktualizowany skład produktu. Pole opcjonalne.
		/// </summary>
		public string? Ingredients { get; init; }

		/// <summary>
		/// Zaktualizowany kod kreskowy produktu (EAN/UPC). Pole opcjonalne.
		/// </summary>
		public string? Barcode { get; init; }

		/// <summary>
		/// Zaktualizowana kategoria produktu z enumu ProductCategory.
		/// </summary>
		public ProductCategory Category { get; init; }

		/// <summary>
		/// Zaktualizowana jednostka miary produktu z enumu ProductUnit.
		/// </summary>
		public ProductUnit Unit { get; init; }

		/// <summary>
		/// Zaktualizowana wielkość porcji produktu w podanych jednostkach.
		/// </summary>
		public float ServingSize { get; init; }

		/// <summary>
		/// Zaktualizowana ilość kalorii na 100g/ml produktu.
		/// </summary>
		public float CaloriesPer100g { get; init; }

		/// <summary>
		/// Zaktualizowana ilość białka na 100g/ml produktu.
		/// </summary>
		public float ProteinPer100g { get; init; }

		/// <summary>
		/// Zaktualizowana ilość tłuszczu na 100g/ml produktu.
		/// </summary>
		public float FatPer100g { get; init; }

		/// <summary>
		/// Zaktualizowana ilość węglowodanów na 100g/ml produktu.
		/// </summary>
		public float CarbohydratesPer100g { get; init; }

		/// <summary>
		/// Zaktualizowana ilość błonnika na 100g/ml produktu. Pole opcjonalne.
		/// </summary>
		public float? FiberPer100g { get; init; }

		/// <summary>
		/// Zaktualizowana ilość cukrów na 100g/ml produktu. Pole opcjonalne.
		/// </summary>
		public float? SugarsPer100g { get; init; }

		/// <summary>
		/// Zaktualizowana ilość sodu na 100g/ml produktu. Pole opcjonalne.
		/// </summary>
		public float? SodiumPer100g { get; init; }

		/// <summary>
		/// Identyfikator użytkownika modyfikującego produkt. Pole wymagane.
		/// </summary>
		public string UpdatedByUserId { get; init; } = null!;
	}
}