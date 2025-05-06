using CalorieTracker.Api.Models.Auth;
using FluentValidation;

namespace CalorieTracker.Api.Validation
{
	public class LoginRequestValidator : AbstractValidator<LoginRequest>
	{
		public LoginRequestValidator()
		{
			RuleFor(x => x.Email)
				.NotEmpty().WithMessage("Email jest wymagany")
				.EmailAddress().WithMessage("Nieprawidłowy format e‑mail");
			RuleFor(x => x.Password)
				.NotEmpty().WithMessage("Hasło jest wymagane")
				.MinimumLength(8).WithMessage("Hasło musi mieć min. 8 znaków");
		}
	}
}
