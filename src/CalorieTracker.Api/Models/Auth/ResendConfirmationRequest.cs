// Plik ResendConfirmationRequest.cs - model żądania ponownego wysłania e-maila potwierdzającego.
// Odpowiada za przechwytywanie adresu e-mail wymaganego do ponownego wysłania wiadomości potwierdzającej rejestrację.

namespace CalorieTracker.Api.Models.Auth
{
	/// <summary>
	/// Model żądania ponownego wysłania e-maila potwierdzającego rejestrację użytkownika.
	/// Używany gdy użytkownik nie otrzymał lub utracił pierwotną wiadomość potwierdzającą.
	/// </summary>
	public class ResendConfirmationRequest
	{
		/// <summary>
		/// Adres e-mail użytkownika, na który ma zostać wysłana ponowna wiadomość potwierdzająca.
		/// Wymagany, nie może być wartością null.
		/// </summary>
		public string Email { get; set; } = null!;
	}
}