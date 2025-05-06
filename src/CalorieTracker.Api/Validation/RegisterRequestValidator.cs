using CalorieTracker.Api.Models.Auth;
using FluentValidation;

namespace CalorieTracker.Api.Validation
{
	public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
	{
		public RegisterRequestValidator()
		{
			RuleFor(x => x.Email)
				.NotEmpty().WithMessage("Email jest wymagany")
				.EmailAddress().WithMessage("Nieprawidłowy format e‑mail");

			RuleFor(x => x.Password)
				.NotEmpty().WithMessage("Hasło jest wymagane")
				.MinimumLength(8).WithMessage("Hasło musi mieć min. 8 znaków");

			RuleFor(x => x.FirstName)
				.MaximumLength(50).WithMessage("Imię może mieć max 50 znaków");

			RuleFor(x => x.LastName)
				.MaximumLength(50).WithMessage("Nazwisko może mieć max 50 znaków");

			// Regex do walidacji hasła
			RuleFor(x => x.Password)
				.Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,}$")
				.WithMessage("Hasło musi mieć min. 8 znaków i zawierać małą, wielką literę, cyfrę oraz znak specjalny.");
		}
	}
}
