using CalorieTracker.Api.Models.Recipes;
using FluentValidation;

namespace CalorieTracker.Api.Validation
{
	/// <summary>
	/// Walidator dla aktualizacji przepisu
	/// </summary>
	public class UpdateRecipeRequestValidator : AbstractValidator<UpdateRecipeRequest>
	{
		public UpdateRecipeRequestValidator()
		{
			RuleFor(x => x.Name)
				.NotEmpty().WithMessage("Nazwa przepisu jest wymagana")
				.MaximumLength(200).WithMessage("Nazwa może mieć maksymalnie 200 znaków");

			RuleFor(x => x.Instructions)
				.NotEmpty().WithMessage("Instrukcje przygotowania są wymagane")
				.MaximumLength(5000).WithMessage("Instrukcje mogą mieć maksymalnie 5000 znaków");

			RuleFor(x => x.ServingsCount)
				.InclusiveBetween(1, 50).WithMessage("Liczba porcji musi być z przedziału 1-50");

			RuleFor(x => x.TotalWeightGrams)
				.InclusiveBetween(1, 50000).WithMessage("Masa musi być z przedziału 1-50000g");

			RuleFor(x => x.PreparationTimeMinutes)
				.InclusiveBetween(1, 1440).WithMessage("Czas przygotowania musi być z przedziału 1-1440 minut");

			RuleFor(x => x.Ingredients)
				.NotEmpty().WithMessage("Przepis musi zawierać przynajmniej jeden składnik")
				.Must(ingredients => ingredients.Count <= 50)
				.WithMessage("Przepis może zawierać maksymalnie 50 składników");

			RuleForEach(x => x.Ingredients).SetValidator(new CreateRecipeIngredientRequestValidator());
		}
	}
}