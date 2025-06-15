// Plik RefreshRequest.cs - model żądania odświeżenia tokenów autentykacyjnych.
// Odpowiada za przechwytywanie danych wymaganych do procesu odnowienia tokenu dostępu i odświeżającego.

namespace CalorieTracker.Api.Models.Auth
{
	/// <summary>
	/// Model żądania odświeżenia tokenów uwierzytelniających użytkownika.
	/// Zawiera zarówno wygasający token dostępu jak i token odświeżający potrzebny do generacji nowej pary tokenów.
	/// </summary>
	public class RefreshRequest
	{
		/// <summary>
		/// Aktualny token dostępu użytkownika, który wygasa lub stał się nieaktualny.
		/// Wymagany, nie może być wartością null.
		/// </summary>
		public string AccessToken { get; set; } = null!;

		/// <summary>
		/// Token odświeżający powiązany z daną sesją użytkownika.
		/// Wymagany, nie może być wartością null.
		/// </summary>
		public string RefreshToken { get; set; } = null!;
	}
}