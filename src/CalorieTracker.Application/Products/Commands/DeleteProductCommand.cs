// Plik DeleteProductCommand.cs - definicja komendy usuwania produktu.
// Przenosi dane wymagane do usunięcia istniejącego produktu z systemu.

namespace CalorieTracker.Application.Auth.Commands
{
	/// <summary>
	/// Rekord reprezentujący komendę usuwania produktu spożywczego.
	/// Zawiera identyfikator produktu oraz informację o użytkowniku wykonującym operację.
	/// </summary>
	/// <param name="Id">Unikalny identyfikator produktu do usunięcia.</param>
	/// <param name="DeletedByUserId">Identyfikator użytkownika wykonującego operację usunięcia.</param>
	public record DeleteProductCommand(Guid Id, string DeletedByUserId);
}