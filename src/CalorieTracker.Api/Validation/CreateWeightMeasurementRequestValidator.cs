// Plik CreateWeightMeasurementRequestValidator.cs - implementacja walidatora tworzenia pomiaru wagi
// Odpowiada za walidację danych nowego pomiaru masy ciała przed dodaniem do systemu

using CalorieTracker.Api.Models.WeightMeasurements;
using FluentValidation;

namespace CalorieTracker.Api.Validation
{
	/// <summary>
	/// Walidator dla żądania utworzenia nowego pomiaru wagi
	/// Sprawdza poprawność danych pomiaru zgodnie z zasadami biznesowymi
	/// </summary>
	public class CreateWeightMeasurementRequestValidator : AbstractValidator<CreateWeightMeasurementRequest>
	{
		/// <summary>
		/// Inicjalizuje nową instancję walidatora pomiaru wagi
		/// Definiuje reguły walidacji dla wszystkich pól pomiaru
		/// </summary>
		public CreateWeightMeasurementRequestValidator()
		{
			// Data pomiaru jest wymagana i nie może być z przyszłości
			RuleFor(x => x.MeasurementDate)
				.NotEmpty().WithMessage("Data pomiaru jest wymagana")
				.LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
				.WithMessage("Data pomiaru nie może być z przyszłości");

			// Waga musi być w realistycznym zakresie
			RuleFor(x => x.WeightKg)
				.InclusiveBetween(20f, 500f)
				.WithMessage("Waga musi być z przedziału 20-500 kg");
		}
	}
}