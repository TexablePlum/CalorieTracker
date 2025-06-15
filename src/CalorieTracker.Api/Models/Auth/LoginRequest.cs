// Plik LoginRequest.cs - model żądania logowania użytkownika.
// Odpowiada za przechwytywanie i walidację danych uwierzytelniających użytkownika podczas procesu logowania.

namespace CalorieTracker.Api.Models.Auth
{
	/// <summary>
	/// Model żądania logowania użytkownika do systemu.
	/// Przechowuje dane uwierzytelniające wymagane do procesu logowania.
	/// </summary>
	public class LoginRequest
	{
		/// <summary>
		/// Adres e-mail użytkownika wykorzystywany do logowania.
		/// Wymagany, nie może być wartością null.
		/// </summary>
		public string Email { get; set; } = null!;

		/// <summary>
		/// Hasło użytkownika wykorzystywane do logowania.
		/// Wymagane, nie może być wartością null.
		/// </summary>
		public string Password { get; set; } = null!;
	}
}