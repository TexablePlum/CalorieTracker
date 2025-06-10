using CalorieTracker.Api.Models.Recipes;
using FluentValidation;

namespace CalorieTracker.Api.Validation
{
	/// <summary>
	/// Walidator dla wyszukiwania przepisów
	/// </summary>
	public class SearchRecipesRequestValidator : AbstractValidator<SearchRecipesRequest>
	{
		public SearchRecipesRequestValidator()
		{
			RuleFor(x => x.SearchTerm)
				.NotEmpty().WithMessage("Fraza wyszukiwania jest wymagana")
				.MinimumLength(2).WithMessage("Fraza wyszukiwania musi mieć minimum 2 znaki")
				.MaximumLength(100).WithMessage("Fraza wyszukiwania może mieć maksymalnie 100 znaków");

			RuleFor(x => x.Skip)
				.GreaterThanOrEqualTo(0).WithMessage("Skip musi być >= 0");

			RuleFor(x => x.Take)
				.InclusiveBetween(1, 100).WithMessage("Take musi być z przedziału 1-100");
		}
	}
}