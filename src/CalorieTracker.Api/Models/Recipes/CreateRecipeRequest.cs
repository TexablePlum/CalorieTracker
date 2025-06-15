// Plik Recipes.cs - model transferu danych (DTO) związany z dodawaniem przepisów.
// Odpowiada za operacje tworzenia przepisów w aplikacji.
namespace CalorieTracker.Api.Models.Recipes
{
	/// <summary>
	/// Model transferu danych (DTO) do tworzenia nowego przepisu.
	/// Zawiera wszystkie wymagane dane do utworzenia przepisu w systemie.
	/// </summary>
	public class CreateRecipeRequest
	{
		/// <summary>
		/// Nazwa przepisu. Pole wymagane.
		/// </summary>
		public string Name { get; set; } = null!;

		/// <summary>
		/// Instrukcje przygotowania przepisu. Pole wymagane.
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
		/// Lista składników wchodzących w skład przepisu.
		/// Domyślnie pusta lista.
		/// </summary>
		public List<CreateRecipeIngredientRequest> Ingredients { get; set; } = new();
	}
}