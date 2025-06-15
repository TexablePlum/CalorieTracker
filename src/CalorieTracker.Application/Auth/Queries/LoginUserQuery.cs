// Plik LoginUserQuery.cs - definicja zapytania logowania użytkownika.
// Przenosi dane uwierzytelniające wymagane do procesu logowania w systemie.

namespace CalorieTracker.Application.Auth.Queries
{
	/// <summary>
	/// Rekord reprezentujący zapytanie logowania użytkownika.
	/// Zawiera dane uwierzytelniające wymagane do procesu logowania.
	/// </summary>
	public class LoginUserQuery
	{
		/// <summary>
		/// Adres email użytkownika wykorzystywany do logowania.
		/// </summary>
		public string Email { get; init; } = null!;

		/// <summary>
		/// Hasło użytkownika wymagane do uwierzytelnienia.
		/// </summary>
		public string Password { get; init; } = null!;
	}
}