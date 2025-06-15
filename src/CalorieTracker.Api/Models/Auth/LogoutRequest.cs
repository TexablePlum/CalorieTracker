// Plik LogoutRequest.cs - model żądania wylogowania użytkownika.
// Odpowiada za przechwytywanie tokena odświeżającego wymaganego do poprawnego zakończenia sesji użytkownika.

namespace CalorieTracker.Api.Models.Auth
{
	/// <summary>
	/// Model żądania wylogowania użytkownika z systemu.
	/// Zawiera token odświeżający potrzebny do unieważnienia sesji użytkownika.
	/// </summary>
	public class LogoutRequest
	{
		/// <summary>
		/// Token odświeżający użytkownika, który ma zostać unieważniony.
		/// Wymagany, nie może być wartością null.
		/// </summary>
		public string RefreshToken { get; set; } = null!;
	}
}