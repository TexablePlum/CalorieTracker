using CalorieTracker.Api.Models.NutritionTracking;
using FluentValidation;

namespace CalorieTracker.Api.Validation.NutritionTracking
{
	/// <summary>
	/// Walidator dla modelu UpdateWaterIntakeRequest.
	/// Sprawdza poprawność danych przy aktualizacji wpisu spożycia wody.
	/// </summary>
	public class UpdateWaterIntakeRequestValidator : AbstractValidator<UpdateWaterIntakeRequest>
	{
		/// <summary>
		/// Inicjalizuje nową instancję walidatora dla UpdateWaterIntakeRequest.
		/// </summary>
		public UpdateWaterIntakeRequestValidator()
		{
			// Walidacja ilości wody
			RuleFor(x => x.AmountMilliliters)
				.GreaterThan(0)
				.WithMessage("Ilość wody musi być większa niż 0ml")
				.LessThanOrEqualTo(5000)
				.WithMessage("Ilość wody nie może przekraczać 5000ml (5 litrów)")
				.Must(BeReasonableAmount)
				.WithMessage("Podana ilość wody wydaje się nierealistyczna");

			// Walidacja czasu spożycia
			RuleFor(x => x.ConsumedAt)
				.NotEmpty()
				.WithMessage("Czas spożycia jest wymagany")
				.LessThanOrEqualTo(DateTime.UtcNow.AddMinutes(5))
				.WithMessage("Czas spożycia nie może być w przyszłości");

			// Walidacja notatek (opcjonalna)
			RuleFor(x => x.Notes)
				.MaximumLength(200)
				.WithMessage("Notatki nie mogą przekraczać 200 znaków")
				.When(x => !string.IsNullOrEmpty(x.Notes));
		}

		/// <summary>
		/// Sprawdza czy podana ilość wody jest rozsądna.
		/// </summary>
		private static bool BeReasonableAmount(float amount)
		{
			return amount >= 10 && amount <= 3000;
		}
	}
}