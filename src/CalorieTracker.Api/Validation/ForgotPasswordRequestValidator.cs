using FluentValidation;
using CalorieTracker.Api.Models.Auth;

namespace CalorieTracker.Api.Validation
{
	public class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
	{
		public ForgotPasswordRequestValidator()
		{
			RuleFor(x => x.Email)
				.NotEmpty().WithMessage("E-mail jest wymagany")
				.EmailAddress().WithMessage("Niepoprawny format e-maila");
		}
	}

}
