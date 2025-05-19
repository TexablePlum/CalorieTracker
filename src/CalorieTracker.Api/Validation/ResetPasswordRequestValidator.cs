using CalorieTracker.Api.Models.Auth;
using FluentValidation;

namespace CalorieTracker.Api.Validation
{
	public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
	{
		public ResetPasswordRequestValidator()
		{
			RuleFor(x => x.Email)
				.NotEmpty().WithMessage("E-mail jest wymagany")
				.EmailAddress().WithMessage("Nieprawidłowy format e-maila");

			RuleFor(x => x.Code)
				.NotEmpty().WithMessage("Kod resetujący jest wymagany")
				.Length(6).WithMessage("Kod musi składać się z 6 cyfr");

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
