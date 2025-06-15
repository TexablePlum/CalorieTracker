// Plik CreateRecipeRequestValidator.cs - implementacja walidatora dla tworzenia przepisu
// Odpowiada za walidację danych nowego przepisu przed dodaniem do systemu

using CalorieTracker.Api.Models.Recipes;
using FluentValidation;

namespace CalorieTracker.Api.Validation
{
	/// <summary>
	/// Walidator dla żądania utworzenia nowego przepisu
	/// Sprawdza poprawność wszystkich pól przepisu i jego składników
	/// </summary>
	public class CreateRecipeRequestValidator : AbstractValidator<CreateRecipeRequest>
	{
		/// <summary>
		/// Inicjalizuje nową instancję walidatora przepisu
		/// Definiuje kompleksowe reguły walidacji dla wszystkich właściwości przepisu
		/// </summary>
		public CreateRecipeRequestValidator()
		{
			// Nazwa przepisu jest wymagana i nie może przekraczać 200 znaków
			RuleFor(x => x.Name)
				.NotEmpty().WithMessage("Nazwa przepisu jest wymagana")
				.MaximumLength(200).WithMessage("Nazwa może mieć maksymalnie 200 znaków");

			// Instrukcje przygotowania są wymagane i nie mogą przekraczać 5000 znaków
			RuleFor(x => x.Instructions)
				.NotEmpty().WithMessage("Instrukcje przygotowania są wymagane")
				.MaximumLength(5000).WithMessage("Instrukcje mogą mieć maksymalnie 5000 znaków");

			// Liczba porcji musi być w odpowiednim zakresie
			RuleFor(x => x.ServingsCount)
				.InclusiveBetween(1, 50).WithMessage("Liczba porcji musi być z przedziału 1-50");

			// Całkowita masa przepisu musi być w odpowiednim zakresie
			RuleFor(x => x.TotalWeightGrams)
				.InclusiveBetween(1, 50000).WithMessage("Masa musi być z przedziału 1-50000g");

			// Czas przygotowania musi być w odpowiednim zakresie
			RuleFor(x => x.PreparationTimeMinutes)
				.InclusiveBetween(1, 1440).WithMessage("Czas przygotowania musi być z przedziału 1-1440 minut");

			// Przepis musi zawierać składniki (1-50)
			RuleFor(x => x.Ingredients)
				.NotEmpty().WithMessage("Przepis musi zawierać przynajmniej jeden składnik")
				.Must(ingredients => ingredients.Count <= 50)
				.WithMessage("Przepis może zawierać maksymalnie 50 składników");

			// Każdy składnik musi być zwalidowany przez odpowiedni walidator
			RuleForEach(x => x.Ingredients)
				.SetValidator(new CreateRecipeIngredientRequestValidator());
		}
	}
}