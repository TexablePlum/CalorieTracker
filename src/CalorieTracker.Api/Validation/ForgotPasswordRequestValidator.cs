// Plik ForgotPasswordRequestValidator.cs - implementacja walidatora żądania resetu hasła
// Odpowiada za walidację danych żądania przypomnienia hasła użytkownika

using FluentValidation;
using CalorieTracker.Api.Models.Auth;

namespace CalorieTracker.Api.Validation
{
	/// <summary>
	/// Walidator dla żądania resetu hasła
	/// Sprawdza poprawność adresu email wymaganego do procesu resetowania hasła
	/// </summary>
	public class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
	{
		/// <summary>
		/// Inicjalizuje nową instancję walidatora resetu hasła
		/// Definiuje reguły walidacji dla pola email
		/// </summary>
		public ForgotPasswordRequestValidator()
		{
			// Email jest wymagany i musi mieć poprawny format
			RuleFor(x => x.Email)
				.NotEmpty().WithMessage("E-mail jest wymagany")
				.EmailAddress().WithMessage("Niepoprawny format e-maila");
		}
	}
}