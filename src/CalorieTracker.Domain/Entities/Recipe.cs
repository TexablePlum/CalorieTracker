// Plik Recipe.cs - definicja encji przepisu kulinarnego.
// Reprezentuje szczegółowe informacje o przepisie, włączając instrukcje, dane o porcjach i metadane.

using System.ComponentModel.DataAnnotations;
using CalorieTracker.Domain.Entities;

namespace CalorieTracker.Domain.Entities
{
	/// &lt;summary>
	/// Encja reprezentująca przepis kulinarny w bazie danych.
	/// Zawiera podstawowe informacje o przepisie, takie jak nazwa, instrukcje, liczba porcji,
	/// waga całkowita, czas przygotowania oraz metadane.
	/// &lt;/summary>
	public class Recipe
	{
		/// &lt;summary>
		/// Unikalny identyfikator GUID przepisu.
		/// Jest kluczem głównym encji.
		/// &lt;/summary>
		[Key]
		public Guid Id { get; set; } = Guid.NewGuid();

		/// <summary>
		/// Nazwa przepisu (np. "Omlet z warzywami").
		/// Pole wymagane, maksymalna długość 200 znaków.
		/// </summary>
		[Required]
		[MaxLength(200)]
		public string Name { get; set; } = null!;

		/// <summary>
		/// Instrukcje przygotowania potrawy.
		/// Pole wymagane, maksymalna długość 5000 znaków.
		/// </summary>
		[Required]
		[MaxLength(5000)]
		public string Instructions { get; set; } = null!;

		/// <summary>
		/// Liczba porcji, na które przeznaczony jest przepis.
		/// Pole wymagane, wartość w zakresie od 1 do 50.
		/// </summary>
		[Required]
		[Range(1, 50)]
		public int ServingsCount { get; set; }

		/// <summary>
		/// Całkowita masa gotowej potrawy w gramach.
		/// Pole wymagane, wartość w zakresie od 1 do 50000.
		/// </summary>
		[Required]
		[Range(1, 50000)]
		public float TotalWeightGrams { get; set; }

		/// <summary>
		/// Czas przygotowania potrawy w minutach.
		/// Pole wymagane, wartość w zakresie od 1 do 1440 (maksymalnie 24 godziny).
		/// </summary>
		[Required]
		[Range(1, 1440)] // max 24h
		public int PreparationTimeMinutes { get; set; }

		// === METADANE ===

		/// <summary>
		/// Identyfikator użytkownika, który dodał przepis.
		/// Pole wymagane.
		/// </summary>
		[Required]
		public string CreatedByUserId { get; set; } = null!;

		/// <summary>
		/// Data i czas utworzenia przepisu.
		/// Domyślnie ustawiane na aktualny czas UTC.
		/// </summary>
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		/// <summary>
		/// Data i czas ostatniej modyfikacji przepisu.
		/// Domyślnie ustawiane na aktualny czas UTC.
		/// </summary>
		public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

		// === RELACJE ===

		/// <summary>
		/// Właściwość nawigacyjna do użytkownika <see cref="ApplicationUser"/>, który utworzył przepis.
		/// </summary>
		public ApplicationUser CreatedByUser { get; set; } = null!;

		/// <summary>
		/// Lista składników <see cref="RecipeIngredient"/> należących do tego przepisu.
		/// Reprezentuje kolekcję powiązanych składników.
		/// </summary>
		public List<RecipeIngredient> Ingredients { get; set; } = new();
	}
}