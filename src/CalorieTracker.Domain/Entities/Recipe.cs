using System.ComponentModel.DataAnnotations;
using CalorieTracker.Domain.Entities;

namespace CalorieTracker.Domain.Entities
{
	/// <summary>
	/// Encja reprezentująca przepis kulinarny w bazie danych
	/// </summary>
	public class Recipe
	{
		[Key]
		public Guid Id { get; set; } = Guid.NewGuid();

		/// <summary>Nazwa przepisu (np. "Omlet z warzywami")</summary>
		[Required]
		[MaxLength(200)]
		public string Name { get; set; } = null!;

		/// <summary>Instrukcje przygotowania potrawy</summary>
		[Required]
		[MaxLength(5000)]
		public string Instructions { get; set; } = null!;

		/// <summary>Na ile porcji przeznaczony jest przepis</summary>
		[Required]
		[Range(1, 50)]
		public int ServingsCount { get; set; }

		/// <summary>Całkowita masa gotowej potrawy w gramach</summary>
		[Required]
		[Range(1, 50000)]
		public float TotalWeightGrams { get; set; }

		/// <summary>Czas przygotowania w minutach</summary>
		[Required]
		[Range(1, 1440)] // max 24h
		public int PreparationTimeMinutes { get; set; }

		// === METADANE ===

		/// <summary>ID użytkownika który dodał przepis</summary>
		[Required]
		public string CreatedByUserId { get; set; } = null!;

		/// <summary>Data utworzenia przepisu</summary>
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		/// <summary>Data ostatniej modyfikacji</summary>
		public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

		// === RELACJE ===

		/// <summary>Użytkownik który utworzył przepis</summary>
		public ApplicationUser CreatedByUser { get; set; } = null!;

		/// <summary>Lista składników przepisu</summary>
		public List<RecipeIngredient> Ingredients { get; set; } = new();
	}
}