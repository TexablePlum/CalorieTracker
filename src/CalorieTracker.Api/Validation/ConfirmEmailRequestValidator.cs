// Plik ConfirmEmailRequestValidator.cs - implementacja walidatora dla potwierdzenia adresu email
// Odpowiada za walidację danych żądania potwierdzenia adresu email użytkownika

using CalorieTracker.Api.Models.Auth;
using FluentValidation;

namespace CalorieTracker.Api.Validation
{
	/// <summary>
	/// Klasa walidatora dla żądania potwierdzenia adresu email
	/// Sprawdza poprawność formatu emaila i kodu potwierdzającego
	/// </summary>
	public class ConfirmEmailRequestValidator : AbstractValidator<ConfirmEmailRequest>
	{
		/// <summary>
		/// Inicjalizuje nową instancję walidatora
		/// Definiuje reguły walidacji dla pól żądania
		/// </summary>
		public ConfirmEmailRequestValidator()
		{
			// Email musi być niepusty i mieć poprawny format
			RuleFor(x => x.Email)
				.NotEmpty()
				.EmailAddress();

			// Kod musi być niepusty i mieć dokładnie 6 znaków
			RuleFor(x => x.Code)
				.NotEmpty()
				.Length(6, 6)
				.WithMessage("Kod musi mieć dokładnie 6 cyfr");
		}
	}
}