// Plik ResetPasswordRequest.cs - model żądania resetowania hasła użytkownika.
// Odpowiada za przechwytywanie danych wymaganych do procesu zmiany hasła użytkownika po weryfikacji.

namespace CalorieTracker.Api.Models.Auth
{
	/// <summary>
	/// Model żądania resetowania hasła użytkownika w systemie.
	/// Zawiera wszystkie dane niezbędne do bezpiecznej zmiany hasła użytkownika.
	/// </summary>
	public class ResetPasswordRequest
	{
		/// <summary>
		/// Adres e-mail użytkownika, dla którego resetowane jest hasło.
		/// Wymagany, nie może być wartością null.
		/// </summary>
		public string Email { get; set; } = null!;

		/// <summary>
		/// Unikalny kod weryfikacyjny wysłany do użytkownika.
		/// Wymagany, nie może być wartością null.
		/// </summary>
		public string Code { get; set; } = null!;

		/// <summary>
		/// Nowe hasło użytkownika, które zastąpi obecne hasło.
		/// Wymagane, nie może być wartością null.
		/// </summary>
		public string NewPassword { get; set; } = null!;
	}
}