// Plik ResetPasswordRequestValidator.cs - implementacja walidatora dla żądania resetowania hasła.
// Odpowiada za walidację danych wejściowych podczas procesu resetowania hasła użytkownika.

using CalorieTracker.Api.Models.Auth;
using FluentValidation;

namespace CalorieTracker.Api.Validation
{
	/// <summary>
	/// Implementacja walidatora FluentValidation dla klasy <see cref="ResetPasswordRequest"/>.
	/// Sprawdza poprawność danych wymaganych do zresetowania hasła użytkownika.
	/// </summary>
	public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
	{
		/// <summary>
		/// Inicjalizuje nową instancję klasy <see cref="ResetPasswordRequestValidator"/>.
		/// Definiuje reguły walidacji dla procesu resetowania hasła.
		/// </summary>
		public ResetPasswordRequestValidator()
		{
			// Reguła walidacji dla pola Email
			RuleFor(x => x.Email)
				.NotEmpty().WithMessage("E-mail jest wymagany")
				.EmailAddress().WithMessage("Nieprawidłowy format e-maila");

			// Reguła walidacji dla pola Code (kod resetujący)
			RuleFor(x => x.Code)
				.NotEmpty().WithMessage("Kod resetujący jest wymagany")
				.Length(6).WithMessage("Kod musi składać się z 6 cyfr");

			// Kompleksowa reguła walidacji dla nowego hasła
			RuleFor(x => x.NewPassword)
				.NotEmpty().WithMessage("Nowe hasło jest wymagane")
				.MinimumLength(8).WithMessage("Hasło musi mieć co najmniej 8 znaków")
				.Matches("[A-Z]").WithMessage("Hasło musi zawierać wielką literę")
				.Matches("[a-z]").WithMessage("Hasło musi zawierać małą literę")
				.Matches("[0-9]").WithMessage("Hasło musi zawierać cyfrę")
				.Matches("[^a-zA-Z0-9]").WithMessage("Hasło musi zawierać znak specjalny");
		}
	}
}