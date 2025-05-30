using CalorieTracker.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CalorieTracker.Domain.Entities
{
	/// <summary>
	/// Encja reprezentująca produkt spożywczy w bazie danych
	/// </summary>
	public class Product
	{
		[Key]
		public Guid Id { get; set; } = Guid.NewGuid();

		/// <summary>Nazwa produktu (np. "Jajko kurze", "Mleko 3,2%")</summary>
		[Required]
		[MaxLength(200)]
		public string Name { get; set; } = null!;

		/// <summary>Marka produktu (opcjonalna)</summary>
		[MaxLength(100)]
		public string? Brand { get; set; }

		/// <summary>Opis produktu</summary>
		[MaxLength(1000)]
		public string? Description { get; set; }

		/// <summary>Skład produktu</summary>
		[MaxLength(2000)]
		public string? Ingredients { get; set; }

		/// <summary>Kod kreskowy (opcjonalny)</summary>
		[MaxLength(50)]
		public string? Barcode { get; set; }

		/// <summary>Kategoria produktu</summary>
		[Required]
		public ProductCategory Category { get; set; }

		/// <summary>Jednostka miary produktu</summary>
		[Required]
		public ProductUnit Unit { get; set; }

		/// <summary>Ilość jednostek na którą odnoszą się wartości odżywcze (np. 100g, 1 sztuka)</summary>
		[Required]
		public float ServingSize { get; set; }

		// === WARTOŚCI ODŻYWCZE NA ServingSize ===

		/// <summary>Kalorie na porcję referencyjną</summary>
		[Required]
		[Range(0, 9999)]
		public float CaloriesPer100g { get; set; }

		/// <summary>Białko w gramach na porcję referencyjną</summary>
		[Required]
		[Range(0, 999)]
		public float ProteinPer100g { get; set; }

		/// <summary>Tłuszcze w gramach na porcję referencyjną</summary>
		[Required]
		[Range(0, 999)]
		public float FatPer100g { get; set; }

		/// <summary>Węglowodany w gramach na porcję referencyjną</summary>
		[Required]
		[Range(0, 999)]
		public float CarbohydratesPer100g { get; set; }

		/// <summary>Błonnik w gramach na porcję referencyjną (opcjonalny)</summary>
		[Range(0, 999)]
		public float? FiberPer100g { get; set; }

		/// <summary>Cukry w gramach na porcję referencyjną (opcjonalny)</summary>
		[Range(0, 999)]
		public float? SugarsPer100g { get; set; }

		/// <summary>Sód w miligramach na porcję referencyjną (opcjonalny)</summary>
		[Range(0, 99999)]
		public float? SodiumPer100g { get; set; }

		// === METADANE ===

		/// <summary>Czy produkt został zweryfikowany przez administratora</summary>
		public bool IsVerified { get; set; } = false;

		/// <summary>ID użytkownika który dodał produkt (null dla produktów systemowych)</summary>
		public string? CreatedByUserId { get; set; }

		/// <summary>Data utworzenia produktu</summary>
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		/// <summary>Data ostatniej modyfikacji</summary>
		public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

		// === RELACJE ===

		/// <summary>Użytkownik który utworzył produkt</summary>
		public ApplicationUser? CreatedByUser { get; set; }
	}
}