using CalorieTracker.Api.Models.NutritionTracking;
using FluentValidation;

namespace CalorieTracker.Api.Validation.NutritionTracking
{
	/// <summary>
	/// Walidator dla modelu LogWaterIntakeRequest.
	/// Sprawdza poprawność danych przy dodawaniu nowego wpisu spożycia wody.
	/// </summary>
	public class LogWaterIntakeRequestValidator : AbstractValidator<LogWaterIntakeRequest>
	{
		/// <summary>
		/// Inicjalizuje nową instancję walidatora dla LogWaterIntakeRequest.
		/// Definiuje reguły walidacji dla wszystkich właściwości modelu.
		/// </summary>
		public LogWaterIntakeRequestValidator()
		{
			// Walidacja ilości wody
			RuleFor(x => x.AmountMilliliters)
				.GreaterThan(0)
				.WithMessage("Ilość wody musi być większa niż 0ml")
				.LessThanOrEqualTo(5000)
				.WithMessage("Ilość wody nie może przekraczać 5000ml (5 litrów)")
				.Must(BeReasonableAmount)
				.WithMessage("Podana ilość wody wydaje się nierealistyczna");

			// Walidacja czasu spożycia (opcjonalna)
			RuleFor(x => x.ConsumedAt)
				.LessThanOrEqualTo(DateTime.UtcNow.AddMinutes(5))
				.WithMessage("Czas spożycia nie może być w przyszłości")
				.When(x => x.ConsumedAt.HasValue);

			// Walidacja notatek (opcjonalna)
			RuleFor(x => x.Notes)
				.MaximumLength(200)
				.WithMessage("Notatki nie mogą przekraczać 200 znaków")
				.When(x => !string.IsNullOrEmpty(x.Notes));
		}

		/// <summary>
		/// Sprawdza czy podana ilość wody jest rozsądna.
		/// Akceptuje typowe ilości jak: 50ml (łyk), 250ml (szklanka), 500ml (butelka), etc.
		/// </summary>
		private static bool BeReasonableAmount(float amount)
		{
			// Minimalna ilość: 10ml (kilka łyków)
			// Maksymalna ilość: 3000ml (3 litry - bardzo duża ilość ale możliwa)
			return amount >= 10 && amount <= 3000;
		}
	}
}