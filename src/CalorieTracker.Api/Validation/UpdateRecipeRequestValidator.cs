// Plik UpdateRecipeRequestValidator.cs - implementacja walidatora dla żądania aktualizacji przepisu.
// Odpowiada za walidację danych wejściowych podczas aktualizacji informacji o przepisie.

using CalorieTracker.Api.Models.Recipes;
using FluentValidation;

namespace CalorieTracker.Api.Validation
{
	/// <summary>
	/// Implementacja walidatora FluentValidation dla klasy <see cref="UpdateRecipeRequest"/>.
	/// Sprawdza poprawność danych przepisu podczas aktualizacji, w tym nazwy, instrukcji, składników i wartości odżywczych.
	/// </summary>
	public class UpdateRecipeRequestValidator : AbstractValidator<UpdateRecipeRequest>
	{
		/// <summary>
		/// Inicjalizuje nową instancję klasy <see cref="UpdateRecipeRequestValidator"/>.
		/// Definiuje kompleksowe reguły walidacji dla wszystkich pól przepisu podlegających aktualizacji.
		/// </summary>
		public UpdateRecipeRequestValidator()
		{
			// Walidacja podstawowych informacji o przepisie
			RuleFor(x => x.Name)
				.NotEmpty().WithMessage("Nazwa przepisu jest wymagana")
				.MaximumLength(200).WithMessage("Nazwa może mieć maksymalnie 200 znaków");

			// Walidacja instrukcji przygotowania
			RuleFor(x => x.Instructions)
				.NotEmpty().WithMessage("Instrukcje przygotowania są wymagane")
				.MaximumLength(5000).WithMessage("Instrukcje mogą mieć maksymalnie 5000 znaków");

			// Walidacja parametrów porcji
			RuleFor(x => x.ServingsCount)
				.InclusiveBetween(1, 50).WithMessage("Liczba porcji musi być z przedziału 1-50");

			// Walidacja masy całkowitej
			RuleFor(x => x.TotalWeightGrams)
				.InclusiveBetween(1, 50000).WithMessage("Masa musi być z przedziału 1-50000g");

			// Walidacja czasu przygotowania
			RuleFor(x => x.PreparationTimeMinutes)
				.InclusiveBetween(1, 1440).WithMessage("Czas przygotowania musi być z przedziału 1-1440 minut");

			// Kompleksowa walidacja listy składników
			RuleFor(x => x.Ingredients)
				.NotEmpty().WithMessage("Przepis musi zawierać przynajmniej jeden składnik")
				.Must(ingredients => ingredients.Count <= 50)
				.WithMessage("Przepis może zawierać maksymalnie 50 składników");

			// Walidacja każdego składnika z osobna przy użyciu dedykowanego walidatora
			RuleForEach(x => x.Ingredients).SetValidator(new CreateRecipeIngredientRequestValidator());
		}
	}
}