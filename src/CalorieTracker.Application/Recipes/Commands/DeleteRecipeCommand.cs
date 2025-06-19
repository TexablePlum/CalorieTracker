// Plik DeleteRecipeCommand.cs - definicja komendy usuwania przepisu
// Odpowiada za przenoszenie danych wymaganych do usunięcia przepisu z systemu

namespace CalorieTracker.Application.Recipes.Commands
{
	/// <summary>
	/// Rekord reprezentujący komendę usuwania przepisu kulinarnego.
	/// Zawiera identyfikator przepisu oraz informację o użytkowniku wykonującym operację.
	/// </summary>
	/// <param name="Id">
	/// Identyfikator przepisu do usunięcia.
	/// Wartość typu Guid reprezentująca unikalny identyfikator istniejącego przepisu w systemie.
	/// </param>
	/// <param name="DeletedByUserId">
	/// Identyfikator użytkownika wykonującego operację usunięcia.
	/// Ciąg znaków reprezentujący identyfikator użytkownika.
	/// Wymagany do weryfikacji uprawnień do wykonania operacji.
	/// </param>
	/// <remarks>
	/// Wykorzystanie typu record gwarantuje niezmienność danych transferowych.
	/// Komenda powinna być poprzedzona weryfikacją uprawnień użytkownika.
	/// </remarks>
	public record DeleteRecipeCommand(Guid Id, string DeletedByUserId);
}