namespace CalorieTracker.Api.Models.Recipes
{
	/// <summary>
	/// Model transferu danych (DTO) zawierający podstawowe informacje o przepisie.
	/// Używany jako element listy przepisów, zawiera tylko najważniejsze dane.
	/// </summary>
	public class RecipeSummaryDto
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
		/// Całkowita wartość kaloryczna przepisu w kcal.
		/// </summary>
		public float TotalCalories { get; set; }

		/// <summary>
		/// Identyfikator użytkownika, który stworzył przepis.
		/// </summary>
		public string CreatedByUserId { get; set; } = null!;

		/// <summary>
		/// Data i czas utworzenia przepisu w systemie.
		/// </summary>
		public DateTime CreatedAt { get; set; }
	}
}
