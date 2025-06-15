// Plik UpdateWeightMeasurementRequestValidator.cs - implementacja walidatora dla żądania aktualizacji pomiaru wagi.
// Odpowiada za walidację danych wejściowych podczas aktualizacji pomiaru masy ciała użytkownika.

using CalorieTracker.Api.Models.WeightMeasurements;
using FluentValidation;

namespace CalorieTracker.Api.Validation
{
	/// <summary>
	/// Implementacja walidatora FluentValidation dla klasy <see cref="UpdateWeightMeasurementRequest"/>.
	/// Sprawdza poprawność danych pomiaru wagi, w tym daty pomiaru i wartości w kilogramach.
	/// </summary>
	public class UpdateWeightMeasurementRequestValidator : AbstractValidator<UpdateWeightMeasurementRequest>
	{
		/// <summary>
		/// Inicjalizuje nową instancję klasy <see cref="UpdateWeightMeasurementRequestValidator"/>.
		/// Definiuje reguły walidacji dla aktualizacji pomiaru masy ciała.
		/// </summary>
		public UpdateWeightMeasurementRequestValidator()
		{
			// Walidacja daty pomiaru
			RuleFor(x => x.MeasurementDate)
				.NotEmpty().WithMessage("Data pomiaru jest wymagana")
				.LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
				.WithMessage("Data pomiaru nie może być z przyszłości");

			// Walidacja wartości wagi
			RuleFor(x => x.WeightKg)
				.InclusiveBetween(20f, 500f)
				.WithMessage("Waga musi być z przedziału 20-500 kg");
		}
	}
}