// Plik RegisterRequestValidator.cs - implementacja walidatora dla żądania rejestracji użytkownika.
// Odpowiada za walidację danych wejściowych podczas rejestracji nowego użytkownika w systemie.

using CalorieTracker.Api.Models.Auth;
using FluentValidation;

namespace CalorieTracker.Api.Validation
{
	/// <summary>
	/// Implementacja walidatora FluentValidation dla klasy <see cref="RegisterRequest"/>.
	/// Sprawdza poprawność danych rejestracyjnych użytkownika, w tym adresu e-mail, hasła oraz imienia i nazwiska.
	/// </summary>
	public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
	{
		/// <summary>
		/// Inicjalizuje nową instancję klasy <see cref="RegisterRequestValidator"/>.
		/// Konfiguruje reguły walidacji dla poszczególnych pól żądania rejestracji.
		/// </summary>
		public RegisterRequestValidator()
		{
			// Reguła walidacji dla pola Email
			RuleFor(x => x.Email)
				.NotEmpty().WithMessage("Email jest wymagany")
				.EmailAddress().WithMessage("Nieprawidłowy format e‑mail");

			// Podstawowa reguła walidacji dla pola Password
			RuleFor(x => x.Password)
				.NotEmpty().WithMessage("Hasło jest wymagane")
				.MinimumLength(8).WithMessage("Hasło musi mieć min. 8 znaków");

			// Reguła walidacji dla pola FirstName
			RuleFor(x => x.FirstName)
				.MaximumLength(50).WithMessage("Imię może mieć max 50 znaków");

			// Reguła walidacji dla pola LastName
			RuleFor(x => x.LastName)
				.MaximumLength(50).WithMessage("Nazwisko może mieć max 50 znaków");

			// Zaawansowana reguła walidacji dla pola Password z wykorzystaniem wyrażenia regularnego
			RuleFor(x => x.Password)
				.Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,}$")
				.WithMessage("Hasło musi mieć min. 8 znaków i zawierać małą, wielką literę, cyfrę oraz znak specjalny.");
		}
	}
}