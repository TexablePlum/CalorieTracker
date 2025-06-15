// Plik CreateRecipeIngredientRequestValidator.cs - implementacja walidatora składnika przepisu
// Odpowiada za walidację danych składnika dodawanego do przepisu

using CalorieTracker.Api.Models.Recipes;
using FluentValidation;

namespace CalorieTracker.Api.Validation
{
	/// <summary>
	/// Walidator dla żądania dodania składnika do przepisu
	/// Sprawdza poprawność danych składnika przepisu
	/// </summary>
	public class CreateRecipeIngredientRequestValidator : AbstractValidator<CreateRecipeIngredientRequest>
	{
		/// <summary>
		/// Inicjalizuje nową instancję walidatora składnika przepisu
		/// Definiuje reguły walidacji dla pól składnika
		/// </summary>
		public CreateRecipeIngredientRequestValidator()
		{
			// Identyfikator produktu jest wymagany
			RuleFor(x => x.ProductId)
				.NotEmpty().WithMessage("ID produktu jest wymagane");

			// Ilość produktu musi być w odpowiednim zakresie
			RuleFor(x => x.Quantity)
				.InclusiveBetween(0.1f, 10000f).WithMessage("Ilość musi być z przedziału 0.1-10000");
		}
	}
}