// Plik CreateProductRequestValidator.cs - implementacja walidatora dla tworzenia produktu
// Odpowiada za walidację danych żądania dodania nowego produktu do systemu

using CalorieTracker.Api.Models.Products;
using CalorieTracker.Domain.Enums;
using FluentValidation;

namespace CalorieTracker.Api.Validation
{
	/// <summary>
	/// Walidator dla żądania tworzenia nowego produktu
	/// Sprawdza poprawność wszystkich pól produktu zgodnie z wymaganiami biznesowymi
	/// </summary>
	public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
	{
		/// <summary>
		/// Inicjalizuje nową instancję walidatora produktu
		/// Definiuje kompleksowe reguły walidacji dla wszystkich właściwości produktu
		/// </summary>
		public CreateProductRequestValidator()
		{
			// Nazwa produktu jest wymagana i nie może przekraczać 200 znaków
			RuleFor(x => x.Name)
				.NotEmpty().WithMessage("Nazwa produktu jest wymagana")
				.MaximumLength(200).WithMessage("Nazwa może mieć maksymalnie 200 znaków");

			// Marka produktu nie może przekraczać 100 znaków
			RuleFor(x => x.Brand)
				.MaximumLength(100).WithMessage("Marka może mieć maksymalnie 100 znaków");

			// Opis produktu nie może przekraczać 1000 znaków
			RuleFor(x => x.Description)
				.MaximumLength(1000).WithMessage("Opis może mieć maksymalnie 1000 znaków");

			// Skład produktu nie może przekraczać 2000 znaków
			RuleFor(x => x.Ingredients)
				.MaximumLength(2000).WithMessage("Skład może mieć maksymalnie 2000 znaków");

			// Kod kreskowy nie może przekraczać 50 znaków
			RuleFor(x => x.Barcode)
				.MaximumLength(50).WithMessage("Kod kreskowy może mieć maksymalnie 50 znaków");

			// Kategoria produktu jest wymagana i musi być poprawną wartością enum
			RuleFor(x => x.Category)
				.NotEmpty().WithMessage("Kategoria jest wymagana")
				.Must(c => Enum.TryParse<ProductCategory>(c, true, out _))
				.WithMessage("Nieprawidłowa kategoria produktu");

			// Jednostka produktu jest wymagana i musi być poprawną wartością enum
			RuleFor(x => x.Unit)
				.NotEmpty().WithMessage("Jednostka jest wymagana")
				.Must(u => Enum.TryParse<ProductUnit>(u, true, out _))
				.WithMessage("Nieprawidłowa jednostka produktu");

			// Wielkość porcji musi być większa od 0
			RuleFor(x => x.ServingSize)
				.GreaterThan(0).WithMessage("Wielkość porcji musi być większa od 0");

			// Walidacja wartości odżywczych na 100g produktu
			RuleFor(x => x.CaloriesPer100g)
				.InclusiveBetween(0, 9999).WithMessage("Kalorie muszą być z przedziału 0-9999");

			RuleFor(x => x.ProteinPer100g)
				.InclusiveBetween(0, 999).WithMessage("Białko musi być z przedziału 0-999g");

			RuleFor(x => x.FatPer100g)
				.InclusiveBetween(0, 999).WithMessage("Tłuszcze muszą być z przedziału 0-999g");

			RuleFor(x => x.CarbohydratesPer100g)
				.InclusiveBetween(0, 999).WithMessage("Węglowodany muszą być z przedziału 0-999g");

			// Walidacja opcjonalnych wartości odżywczych
			RuleFor(x => x.FiberPer100g)
				.InclusiveBetween(0, 999).WithMessage("Błonnik musi być z przedziału 0-999g")
				.When(x => x.FiberPer100g.HasValue);

			RuleFor(x => x.SugarsPer100g)
				.InclusiveBetween(0, 999).WithMessage("Cukry muszą być z przedziału 0-999g")
				.When(x => x.SugarsPer100g.HasValue);

			RuleFor(x => x.SodiumPer100g)
				.InclusiveBetween(0, 99999).WithMessage("Sód musi być z przedziału 0-99999mg")
				.When(x => x.SodiumPer100g.HasValue);
		}
	}
}