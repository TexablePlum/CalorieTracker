// Plik CreateRecipeCommand.cs - definicja komendy tworzenia nowego przepisu
// Odpowiada za przenoszenie danych wymaganych do utworzenia nowego przepisu kulinarnego

namespace CalorieTracker.Application.Recipes.Commands
{
	/// <summary>
	/// Rekord reprezentujący komendę tworzenia nowego przepisu kulinarnego.
	/// Zawiera wszystkie niezbędne dane do utworzenia przepisu wraz z listą składników.
	/// </summary>
	/// <remarks>
	/// Wykorzystanie typu record gwarantuje niezmienność danych transferowych.
	/// Właściwość Ingredients zawiera listę komend tworzenia poszczególnych składników przepisu.
	/// </remarks>
	public record CreateRecipeCommand
	{
		/// <summary>
		/// Nazwa przepisu
		/// </summary>
		/// <value>
		/// Ciąg znaków reprezentujący nazwę przepisu. Wymagane, nie może być puste.
		/// </value>
		public string Name { get; init; } = null!;

		/// <summary>
		/// Instrukcje przygotowania przepisu
		/// </summary>
		/// <value>
		/// Tekst zawierający kroki przygotowania potrawy. Wymagane, nie może być puste.
		/// </value>
		public string Instructions { get; init; } = null!;

		/// <summary>
		/// Liczba porcji
		/// </summary>
		/// <value>
		/// Liczba całkowita określająca dla ilu osób jest przepis. Wartość musi być dodatnia.
		/// </value>
		public int ServingsCount { get; init; }

		/// <summary>
		/// Całkowita waga przepisu w gramach
		/// </summary>
		/// <value>
		/// Wartość zmiennoprzecinkowa określająca łączną wagę wszystkich składników.
		/// </value>
		public float TotalWeightGrams { get; init; }

		/// <summary>
		/// Czas przygotowania w minutach
		/// </summary>
		/// <value>
		/// Liczba całkowita określająca czas potrzebny na przygotowanie przepisu.
		/// </value>
		public int PreparationTimeMinutes { get; init; }

		/// <summary>
		/// Identyfikator użytkownika tworzącego przepis
		/// </summary>
		/// <value>
		/// Ciąg znaków reprezentujący identyfikator użytkownika. Wymagane.
		/// </value>
		public string CreatedByUserId { get; init; } = null!;

		/// <summary>
		/// Lista składników przepisu
		/// </summary>
		/// <value>
		/// Kolekcja obiektów <see cref="CreateRecipeIngredientCommand"/> reprezentujących składniki.
		/// Domyślnie pusta lista.
		/// </value>
		public List<CreateRecipeIngredientCommand> Ingredients { get; init; } = new();
	}
}