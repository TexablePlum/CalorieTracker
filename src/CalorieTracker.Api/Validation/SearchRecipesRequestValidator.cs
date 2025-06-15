// Plik SearchRecipesRequestValidator.cs - implementacja walidatora dla żądania wyszukiwania przepisów.
// Odpowiada za walidację parametrów wyszukiwania przepisów w systemie.

using CalorieTracker.Api.Models.Recipes;
using FluentValidation;

namespace CalorieTracker.Api.Validation
{
	/// <summary>
	/// Implementacja walidatora FluentValidation dla klasy <see cref="SearchRecipesRequest"/>.
	/// Sprawdza poprawność parametrów wyszukiwania przepisów, w tym frazy wyszukiwania oraz parametrów stronicowania.
	/// </summary>
	public class SearchRecipesRequestValidator : AbstractValidator<SearchRecipesRequest>
	{
		/// <summary>
		/// Inicjalizuje nową instancję klasy <see cref="SearchRecipesRequestValidator"/>.
		/// Definiuje reguły walidacji dla parametrów wyszukiwania przepisów.
		/// </summary>
		public SearchRecipesRequestValidator()
		{
			// Reguła walidacji dla frazy wyszukiwania przepisów
			RuleFor(x => x.SearchTerm)
				.NotEmpty().WithMessage("Fraza wyszukiwania jest wymagana")
				.MinimumLength(2).WithMessage("Fraza wyszukiwania musi mieć minimum 2 znaki")
				.MaximumLength(100).WithMessage("Fraza wyszukiwania może mieć maksymalnie 100 znaków");

			// Reguła walidacji dla parametru Skip (pominięcie rekordów)
			RuleFor(x => x.Skip)
				.GreaterThanOrEqualTo(0).WithMessage("Skip musi być >= 0");

			// Reguła walidacji dla parametru Take (liczba pobieranych rekordów)
			RuleFor(x => x.Take)
				.InclusiveBetween(1, 100).WithMessage("Take musi być z przedziału 1-100");
		}
	}
}