// Plik ConfirmEmailRequest.cs - model żądania potwierdzenia adresu email.
// Definiuje strukturę danych wymaganą do weryfikacji adresu email użytkownika.

namespace CalorieTracker.Api.Models.Auth
{
	/// <summary>
	/// Model żądania potwierdzającego adres email użytkownika.
	/// Zawiera dane niezbędne do weryfikacji i aktywacji konta.
	/// </summary>
	public class ConfirmEmailRequest
	{
		/// <summary>
		/// Adres email użytkownika podlegający weryfikacji.
		/// Musi odpowiadać adresowi użytemu podczas rejestracji.
		/// </summary>
		public string Email { get; set; } = null!;

		/// <summary>
		/// Unikalny kod weryfikacyjny wysłany na adres email użytkownika.
		/// Wymagany do potwierdzenia prawidłowości adresu email.
		/// </summary>
		public string Code { get; set; } = null!;
	}
}