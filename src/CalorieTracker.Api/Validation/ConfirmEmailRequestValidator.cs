using CalorieTracker.Api.Models.Auth;
using FluentValidation;

namespace CalorieTracker.Api.Validation
{
	public class ConfirmEmailRequestValidator : AbstractValidator<ConfirmEmailRequest>
	{
		public ConfirmEmailRequestValidator()
		{
			RuleFor(x => x.Email).NotEmpty().EmailAddress();
			RuleFor(x => x.Code).NotEmpty().Length(6, 6)
				.WithMessage("Kod musi mieć dokładnie 6 cyfr");
		}
	}
}
