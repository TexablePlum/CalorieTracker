using CalorieTracker.Api.Models.Products;
using CalorieTracker.Domain.Enums;
using FluentValidation;

namespace CalorieTracker.Api.Validation
{
	/// <summary>
	/// Walidator dla wyszukiwania produktów
	/// </summary>
	public class SearchProductsRequestValidator : AbstractValidator<SearchProductsRequest>
	{
		public SearchProductsRequestValidator()
		{
			RuleFor(x => x.SearchTerm)
				.NotEmpty().WithMessage("Fraza wyszukiwania jest wymagana")
				.MinimumLength(2).WithMessage("Fraza wyszukiwania musi mieć minimum 2 znaki")
				.MaximumLength(100).WithMessage("Fraza wyszukiwania może mieć maksymalnie 100 znaków");

			RuleFor(x => x.Category)
				.Must(c => string.IsNullOrEmpty(c) || Enum.TryParse<ProductCategory>(c, true, out _))
				.WithMessage("Nieprawidłowa kategoria produktu");

			RuleFor(x => x.Skip)
				.GreaterThanOrEqualTo(0).WithMessage("Skip musi być >= 0");

			RuleFor(x => x.Take)
				.InclusiveBetween(1, 100).WithMessage("Take musi być z przedziału 1-100");
		}
	}
}
