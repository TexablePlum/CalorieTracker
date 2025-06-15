// Plik SearchProductsRequestValidator.cs - implementacja walidatora dla żądania wyszukiwania produktów.
// Odpowiada za walidację parametrów wyszukiwania produktów w systemie.

using CalorieTracker.Api.Models.Products;
using CalorieTracker.Domain.Enums;
using FluentValidation;

namespace CalorieTracker.Api.Validation
{
	/// <summary>
	/// Implementacja walidatora FluentValidation dla klasy <see cref="SearchProductsRequest"/>.
	/// Sprawdza poprawność parametrów wyszukiwania produktów, w tym frazy wyszukiwania, kategorii oraz parametrów stronicowania.
	/// </summary>
	public class SearchProductsRequestValidator : AbstractValidator<SearchProductsRequest>
	{
		/// <summary>
		/// Inicjalizuje nową instancję klasy <see cref="SearchProductsRequestValidator"/>.
		/// Definiuje reguły walidacji dla parametrów wyszukiwania produktów.
		/// </summary>
		public SearchProductsRequestValidator()
		{
			// Reguła walidacji dla frazy wyszukiwania
			RuleFor(x => x.SearchTerm)
				.NotEmpty().WithMessage("Fraza wyszukiwania jest wymagana")
				.MinimumLength(2).WithMessage("Fraza wyszukiwania musi mieć minimum 2 znaki")
				.MaximumLength(100).WithMessage("Fraza wyszukiwania może mieć maksymalnie 100 znaków");

			// Reguła walidacji dla kategorii produktu
			RuleFor(x => x.Category)
				.Must(c => string.IsNullOrEmpty(c) || Enum.TryParse<ProductCategory>(c, true, out _))
				.WithMessage("Nieprawidłowa kategoria produktu");

			// Reguła walidacji dla parametru Skip (pominięcie rekordów)
			RuleFor(x => x.Skip)
				.GreaterThanOrEqualTo(0).WithMessage("Skip musi być >= 0");

			// Reguła walidacji dla parametru Take (liczba pobieranych rekordów)
			RuleFor(x => x.Take)
				.InclusiveBetween(1, 100).WithMessage("Take musi być z przedziału 1-100");
		}
	}
}