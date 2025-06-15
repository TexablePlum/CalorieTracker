// Plik ForgotPasswordRequest.cs - model żądania resetu hasła.
// Odpowiada za przechwytywanie i walidację danych wejściowych dotyczących prośby o reset hasła użytkownika.

namespace CalorieTracker.Api.Models.Auth
{
	/// <summary>
	/// Model żądania resetu hasła użytkownika.
	/// Przechowuje adres e-mail użytkownika, który zgłasza prośbę o reset hasła.
	/// </summary>
	public class ForgotPasswordRequest
	{
		/// <summary>
		/// Adres e-mail użytkownika, dla którego ma zostać zainicjowany proces resetowania hasła.
		/// Wymagany, nie może być wartością null.
		/// </summary>
		public string Email { get; set; } = null!;
	}
}