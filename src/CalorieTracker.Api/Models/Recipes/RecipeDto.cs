// Plik Recipes.cs - model transferu danych (DTO) związany z przepisami.
// Odpowiada za zwracanie szczegółowych informacji o przepisach przez API.
namespace CalorieTracker.Api.Models.Recipes
{
	/// <summary>
	/// Model transferu danych (DTO) zawierający pełne informacje o przepisie.
	/// Używany jako odpowiedź API dla operacji zwracających szczegóły przepisu.
	/// </summary>
	public class RecipeDto
	{
		/// <summary>
		/// Unikalny identyfikator przepisu w systemie.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Nazwa przepisu. Pole wymagane.
		/// </summary>
		public string Name { get; set; } = null!;

		/// <summary>
		/// Szczegółowe instrukcje przygotowania przepisu. Pole wymagane.
		/// </summary>
		public string Instructions { get; set; } = null!;

		/// <summary>
		/// Liczba porcji, na które przygotowany jest przepis.
		/// </summary>
		public int ServingsCount { get; set; }

		/// <summary>
		/// Całkowita waga przepisu w gramach.
		/// </summary>
		public float TotalWeightGrams { get; set; }

		/// <summary>
		/// Czas przygotowania przepisu w minutach.
		/// </summary>
		public int PreparationTimeMinutes { get; set; }

		/// <summary>
		/// Identyfikator użytkownika, który stworzył przepis.
		/// </summary>
		public string CreatedByUserId { get; set; } = null!;

		/// <summary>
		/// Data i czas utworzenia przepisu w systemie.
		/// </summary>
		public DateTime CreatedAt { get; set; }

		/// <summary>
		/// Lista składników wchodzących w skład przepisu.
		/// Każdy element zawiera szczegóły produktu i jego ilość.
		/// Domyślnie pusta lista.
		/// </summary>
		public List<RecipeIngredientDto> Ingredients { get; set; } = new();

		/// <summary>
		/// Agregowane wartości odżywcze dla całego przepisu.
		/// Zawiera sumę kalorii i makroskładników wszystkich składników.
		/// </summary>
		public RecipeNutritionDto TotalNutrition { get; set; } = null!;
	}
}