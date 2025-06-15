// Plik UpdateRecipeCommand.cs - definicja komendy aktualizacji przepisu
// Odpowiada za przenoszenie danych wymaganych do aktualizacji istniejącego przepisu

namespace CalorieTracker.Application.Recipes.Commands
{
	/// <summary>
	/// Rekord reprezentujący komendę aktualizacji przepisu kulinarnego.
	/// Zawiera wszystkie możliwe do zaktualizowania dane przepisu wraz z listą składników.
	/// </summary>
	/// <remarks>
	/// Wykorzystanie typu record gwarantuje niezmienność danych transferowych.
	/// Przed wykonaniem komendy należy zweryfikować uprawnienia użytkownika (UpdatedByUserId).
	/// </remarks>
	public record UpdateRecipeCommand
	{
		/// <summary>
		/// Identyfikator przepisu do aktualizacji
		/// </summary>
		/// <value>
		/// Wartość typu Guid reprezentująca unikalny identyfikator istniejącego przepisu w systemie.
		/// </value>
		public Guid Id { get; init; }

		/// <summary>
		/// Nowa nazwa przepisu
		/// </summary>
		/// <value>
		/// Ciąg znaków reprezentujący zaktualizowaną nazwę przepisu. Wymagane, nie może być puste.
		/// </value>
		public string Name { get; init; } = null!;

		/// <summary>
		/// Nowe instrukcje przygotowania przepisu
		/// </summary>
		/// <value>
		/// Tekst zawierający zaktualizowane kroki przygotowania potrawy. Wymagane, nie może być puste.
		/// </value>
		public string Instructions { get; init; } = null!;

		/// <summary>
		/// Zaktualizowana liczba porcji
		/// </summary>
		/// <value>
		/// Liczba całkowita określająca dla ilu osób jest przepis. Wartość musi być dodatnia.
		/// </value>
		public int ServingsCount { get; init; }

		/// <summary>
		/// Zaktualizowana całkowita waga przepisu
		/// </summary>
		/// <value>
		/// Wartość zmiennoprzecinkowa określająca łączną wagę wszystkich składników w gramach.
		/// </value>
		public float TotalWeightGrams { get; init; }

		/// <summary>
		/// Zaktualizowany czas przygotowania
		/// </summary>
		/// <value>
		/// Liczba całkowita określająca czas potrzebny na przygotowanie przepisu w minutach.
		/// </value>
		public int PreparationTimeMinutes { get; init; }

		/// <summary>
		/// Identyfikator użytkownika aktualizującego przepis
		/// </summary>
		/// <value>
		/// Ciąg znaków reprezentujący identyfikator użytkownika. Wymagany do weryfikacji uprawnień.
		/// </value>
		public string UpdatedByUserId { get; init; } = null!;

		/// <summary>
		/// Lista składników przepisu
		/// </summary>
		/// <value>
		/// Kolekcja obiektów <see cref="CreateRecipeIngredientCommand"/> reprezentujących zaktualizowaną listę składników.
		/// Domyślnie pusta lista. W przypadku aktualizacji całkowicie zastępuje istniejące składniki.
		/// </value>
		public List<CreateRecipeIngredientCommand> Ingredients { get; init; } = new();
	}
}