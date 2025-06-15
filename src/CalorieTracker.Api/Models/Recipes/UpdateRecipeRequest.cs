namespace CalorieTracker.Api.Models.Recipes
{
	/// <summary>
	/// Model transferu danych (DTO) do aktualizacji istniejącego przepisu.
	/// Zawiera wszystkie możliwe do zaktualizowania dane przepisu.
	/// </summary>
	public class UpdateRecipeRequest
	{
		/// <summary>
		/// Nowa nazwa przepisu. Pole wymagane.
		/// </summary>
		public string Name { get; set; } = null!;

		/// <summary>
		/// Zaktualizowane instrukcje przygotowania przepisu. Pole wymagane.
		/// </summary>
		public string Instructions { get; set; } = null!;

		/// <summary>
		/// Zaktualizowana liczba porcji.
		/// </summary>
		public int ServingsCount { get; set; }

		/// <summary>
		/// Zaktualizowana całkowita waga przepisu w gramach.
		/// </summary>
		public float TotalWeightGrams { get; set; }

		/// <summary>
		/// Zaktualizowany czas przygotowania w minutach.
		/// </summary>
		public int PreparationTimeMinutes { get; set; }

		/// <summary>
		/// Zaktualizowana lista składników przepisu.
		/// Domyślnie pusta lista.
		/// </summary>
		public List<CreateRecipeIngredientRequest> Ingredients { get; set; } = new();
	}
}
