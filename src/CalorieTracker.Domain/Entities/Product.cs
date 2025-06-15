// Plik Product.cs - definicja encji produktu spożywczego.
// Reprezentuje szczegółowe informacje o produkcie, włączając dane odżywcze i metadane.

using CalorieTracker.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CalorieTracker.Domain.Entities
{
	/// &lt;summary>
	/// Encja reprezentująca produkt spożywczy w bazie danych.
	/// Zawiera podstawowe informacje o produkcie, jego wartości odżywcze oraz metadane dotyczące utworzenia.
	/// &lt;/summary>
	public class Product
	{
		/// &lt;summary>
		/// Unikalny identyfikator GUID produktu.
		/// Jest kluczem głównym encji.
		/// &lt;/summary>
		[Key]
		public Guid Id { get; set; } = Guid.NewGuid();

		/// <summary>
		/// Nazwa produktu (np. "Jajko kurze", "Mleko 3,2%").
		/// Pole wymagane, maksymalna długość 200 znaków.
		/// </summary>
		[Required]
		[MaxLength(200)]
		public string Name { get; set; } = null!;

		/// <summary>
		/// Marka produktu (opcjonalna).
		/// Maksymalna długość 100 znaków.
		/// </summary>
		[MaxLength(100)]
		public string? Brand { get; set; }

		/// <summary>
		/// Opis produktu.
		/// Maksymalna długość 1000 znaków.
		/// </summary>
		[MaxLength(1000)]
		public string? Description { get; set; }

		/// <summary>
		/// Skład produktu.
		/// Maksymalna długość 2000 znaków.
		/// </summary>
		[MaxLength(2000)]
		public string? Ingredients { get; set; }

		/// <summary>
		/// Kod kreskowy produktu (opcjonalny).
		/// Maksymalna długość 50 znaków.
		/// </summary>
		[MaxLength(50)]
		public string? Barcode { get; set; }

		/// <summary>
		/// Kategoria produktu, zdefiniowana przez <see cref="ProductCategory"/>.
		/// Pole wymagane.
		/// </summary>
		[Required]
		public ProductCategory Category { get; set; }

		/// <summary>
		/// Jednostka miary produktu, zdefiniowana przez <see cref="ProductUnit"/>.
		/// Pole wymagane.
		/// </summary>
		[Required]
		public ProductUnit Unit { get; set; }

		/// <summary>
		/// Ilość jednostek, na którą odnoszą się wartości odżywcze (np. 100g, 1 sztuka).
		/// Pole wymagane.
		/// </summary>
		[Required]
		public float ServingSize { get; set; }

		// === WARTOŚCI ODŻYWCZE NA ServingSize ===

		/// <summary>
		/// Kalorie na porcję referencyjną (np. 100g lub 1 sztukę).
		/// Pole wymagane, wartość w zakresie od 0 do 9999.
		/// </summary>
		[Required]
		[Range(0, 9999)]
		public float CaloriesPer100g { get; set; }

		/// <summary>
		/// Ilość białka w gramach na porcję referencyjną.
		/// Pole wymagane, wartość w zakresie od 0 do 999.
		/// </summary>
		[Required]
		[Range(0, 999)]
		public float ProteinPer100g { get; set; }

		/// <summary>
		/// Ilość tłuszczów w gramach na porcję referencyjną.
		/// Pole wymagane, wartość w zakresie od 0 do 999.
		/// </summary>
		[Required]
		[Range(0, 999)]
		public float FatPer100g { get; set; }

		/// <summary>
		/// Ilość węglowodanów w gramach na porcję referencyjną.
		/// Pole wymagane, wartość w zakresie od 0 do 999.
		/// </summary>
		[Required]
		[Range(0, 999)]
		public float CarbohydratesPer100g { get; set; }

		/// <summary>
		/// Ilość błonnika w gramach na porcję referencyjną (opcjonalny).
		/// Wartość w zakresie od 0 do 999.
		/// </summary>
		[Range(0, 999)]
		public float? FiberPer100g { get; set; }

		/// <summary>
		/// Ilość cukrów w gramach na porcję referencyjną (opcjonalny).
		/// Wartość w zakresie od 0 do 999.
		/// </summary>
		[Range(0, 999)]
		public float? SugarsPer100g { get; set; }

		/// <summary>
		/// Ilość sodu w miligramach na porcję referencyjną (opcjonalny).
		/// Wartość w zakresie od 0 do 99999.
		/// </summary>
		[Range(0, 99999)]
		public float? SodiumPer100g { get; set; }

		// === METADANE ===

		/// <summary>
		/// Wskazuje, czy produkt został zweryfikowany przez administratora.
		/// Domyślnie `false`.
		/// </summary>
		public bool IsVerified { get; set; } = false;

		/// <summary>
		/// Identyfikator użytkownika, który dodał produkt.
		/// Jest `null` dla produktów systemowych.
		/// </summary>
		public string? CreatedByUserId { get; set; }

		/// <summary>
		/// Data i czas utworzenia produktu.
		/// Domyślnie ustawiane na aktualny czas UTC.
		/// </summary>
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		/// <summary>
		/// Data i czas ostatniej modyfikacji produktu.
		/// Domyślnie ustawiane na aktualny czas UTC.
		/// </summary>
		public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

		// === RELACJE ===

		/// <summary>
		/// Właściwość nawigacyjna do użytkownika <see cref="ApplicationUser"/>, który utworzył produkt.
		/// </summary>
		public ApplicationUser? CreatedByUser { get; set; }
	}
}