// Plik LoginRequestValidator.cs - implementacja walidatora logowania
// Odpowiada za walidację danych logowania użytkownika do systemu

using CalorieTracker.Api.Models.Auth;
using FluentValidation;

namespace CalorieTracker.Api.Validation
{
	/// <summary>
	/// Walidator dla żądania logowania użytkownika
	/// Sprawdza poprawność danych uwierzytelniających zgodnie z wymaganiami systemu
	/// </summary>
	public class LoginRequestValidator : AbstractValidator<LoginRequest>
	{
		/// <summary>
		/// Inicjalizuje nową instancję walidatora logowania
		/// Definiuje reguły walidacji dla pól email i hasła
		/// </summary>
		public LoginRequestValidator()
		{
			// Email jest wymagany i musi mieć poprawny format
			RuleFor(x => x.Email)
				.NotEmpty().WithMessage("Email jest wymagany")
				.EmailAddress().WithMessage("Nieprawidłowy format e‑mail");

			// Hasło jest wymagane i musi spełniać minimalne wymagania długości
			RuleFor(x => x.Password)
				.NotEmpty().WithMessage("Hasło jest wymagane")
				.MinimumLength(8).WithMessage("Hasło musi mieć min. 8 znaków");
		}
	}
}