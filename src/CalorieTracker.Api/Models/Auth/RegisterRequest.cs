// Plik RegisterRequest.cs - model żądania rejestracji nowego użytkownika.
// Odpowiada za przechwytywanie danych wymaganych podczas procesu rejestracji nowego konta użytkownika.

namespace CalorieTracker.Api.Models.Auth
{
	/// <summary>
	/// Model żądania rejestracji nowego użytkownika w systemie.
	/// Zawiera podstawowe dane wymagane do utworzenia nowego konta użytkownika.
	/// </summary>
	public class RegisterRequest
	{
		/// <summary>
		/// Adres e-mail użytkownika wykorzystywany jako login do systemu.
		/// Wymagany, nie może być wartością null.
		/// </summary>
		public string Email { get; set; } = null!;

		/// <summary>
		/// Hasło użytkownika wymagane do uwierzytelniania.
		/// Wymagane, nie może być wartością null.
		/// </summary>
		public string Password { get; set; } = null!;

		/// <summary>
		/// Imię użytkownika (opcjonalne).
		/// Może być wartością null, jeśli użytkownik nie chce podawać imienia.
		/// </summary>
		public string? FirstName { get; set; }

		/// <summary>
		/// Nazwisko użytkownika (opcjonalne).
		/// Może być wartością null, jeśli użytkownik nie chce podawać nazwiska.
		/// </summary>
		public string? LastName { get; set; }
	}
}