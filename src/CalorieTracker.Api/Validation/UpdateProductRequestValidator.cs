// Plik UpdateProductRequestValidator.cs - implementacja walidatora dla żądania aktualizacji produktu.
// Odpowiada za walidację danych wejściowych podczas aktualizacji informacji o produkcie.

using CalorieTracker.Api.Models.Products;
using CalorieTracker.Domain.Enums;
using FluentValidation;

namespace CalorieTracker.Api.Validation
{
	/// <summary>
	/// Implementacja walidatora FluentValidation dla klasy <see cref="UpdateProductRequest"/>.
	/// Sprawdza poprawność danych produktu podczas aktualizacji, w tym nazwy, kategorii, wartości odżywczych i innych parametrów.
	/// </summary>
	public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
	{
		/// <summary>
		/// Inicjalizuje nową instancję klasy <see cref="UpdateProductRequestValidator"/>.
		/// Definiuje kompleksowe reguły walidacji dla wszystkich pól produktu podlegających aktualizacji.
		/// </summary>
		public UpdateProductRequestValidator()
		{
			// Sekcja walidacji podstawowych informacji o produkcie
			RuleFor(x => x.Name)
				.NotEmpty().WithMessage("Nazwa produktu jest wymagana")
				.MaximumLength(200).WithMessage("Nazwa może mieć maksymalnie 200 znaków");

			RuleFor(x => x.Brand)
				.MaximumLength(100).WithMessage("Marka może mieć maksymalnie 100 znaków");

			RuleFor(x => x.Description)
				.MaximumLength(1000).WithMessage("Opis może mieć maksymalnie 1000 znaków");

			RuleFor(x => x.Ingredients)
				.MaximumLength(2000).WithMessage("Skład może mieć maksymalnie 2000 znaków");

			RuleFor(x => x.Barcode)
				.MaximumLength(50).WithMessage("Kod kreskowy może mieć maksymalnie 50 znaków");

			// Walidacja kategorii i jednostek produktu
			RuleFor(x => x.Category)
				.NotEmpty().WithMessage("Kategoria jest wymagana")
				.Must(c => Enum.TryParse<ProductCategory>(c, true, out _))
				.WithMessage("Nieprawidłowa kategoria produktu");

			RuleFor(x => x.Unit)
				.NotEmpty().WithMessage("Jednostka jest wymagana")
				.Must(u => Enum.TryParse<ProductUnit>(u, true, out _))
				.WithMessage("Nieprawidłowa jednostka produktu");

			// Walidacja wielkości porcji
			RuleFor(x => x.ServingSize)
				.GreaterThan(0).WithMessage("Wielkość porcji musi być większa od 0");

			// Sekcja walidacji wartości odżywczych (na 100g)
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