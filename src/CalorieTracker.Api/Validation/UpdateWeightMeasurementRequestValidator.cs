using CalorieTracker.Api.Models.WeightMeasurements;
using FluentValidation;

namespace CalorieTracker.Api.Validation
{
	/// <summary>
	/// Walidator dla aktualizacji pomiaru masy ciała
	/// </summary>
	public class UpdateWeightMeasurementRequestValidator : AbstractValidator<UpdateWeightMeasurementRequest>
	{
		public UpdateWeightMeasurementRequestValidator()
		{
			RuleFor(x => x.MeasurementDate)
				.NotEmpty().WithMessage("Data pomiaru jest wymagana")
				.LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
				.WithMessage("Data pomiaru nie może być z przyszłości");

			RuleFor(x => x.WeightKg)
				.InclusiveBetween(20f, 500f)
				.WithMessage("Waga musi być z przedziału 20-500 kg");
		}
	}
}
