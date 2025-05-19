using CalorieTracker.Api.Models.Profile;
using CalorieTracker.Domain.Enums;
using FluentValidation;

namespace CalorieTracker.Api.Validation
{
	public class UpdateUserProfileRequestValidator : AbstractValidator<UpdateUserProfileRequest>
	{
		public UpdateUserProfileRequestValidator()
		{
			// ---------- Dane konta ----------
			RuleFor(x => x.FirstName)
				.NotEmpty().WithMessage("Imię jest wymagane")
				.MaximumLength(50).WithMessage("Imię może mieć maksymalnie 50 znaków")
				.Matches(@"^[A-Za-zÀ-žżźćńółęąśŻŹĆĄŚĘŁÓŃ\s'-]+$")
				.WithMessage("Imię zawiera niedozwolone znaki");

			RuleFor(x => x.LastName)
				.NotEmpty().WithMessage("Nazwisko jest wymagane")
				.MaximumLength(50).WithMessage("Nazwisko może mieć maksymalnie 50 znaków")
				.Matches(@"^[A-Za-zÀ-žżźćńółęąśŻŹĆĄŚĘŁÓŃ\s'-]+$")
				.WithMessage("Nazwisko zawiera niedozwolone znaki");

			// ---------- Dane profilu ----------
			RuleFor(x => x.Age)
				.NotNull().WithMessage("Wiek jest wymagany")
				.InclusiveBetween(13, 130).WithMessage("Wiek musi być z przedziału 13-130 lat");

			RuleFor(x => x.HeightCm)
				.NotNull().WithMessage("Wzrost jest wymagany")
				.InclusiveBetween(100, 250).WithMessage("Wzrost musi być z przedziału 100–250 cm");

			RuleFor(x => x.WeightKg)
				.NotNull().WithMessage("Waga jest wymagana")
				.InclusiveBetween(30, 300).WithMessage("Waga musi być z przedziału 30–300 kg");

			RuleFor(x => x.Gender)
				.NotEmpty().WithMessage("Płeć jest wymagana")
				.Must(g => Enum.TryParse<Gender>(g, true, out _))
				.WithMessage("Nieprawidłowa wartość płci (dozwolone: Male, Female, Other)");

			RuleFor(x => x.ActivityLevel)
				.NotEmpty().WithMessage("Poziom aktywności jest wymagany")
				.Must(a => Enum.TryParse<ActivityLevel>(a, true, out _))
				.WithMessage("Nieprawidłowy poziom aktywności (dozwolone: Sedentary, LightlyActive, ModeratelyActive, VeryActive, ExtremelyActive)");

			RuleFor(x => x.Goal)
				.NotEmpty().WithMessage("Cel jest wymagany")
				.Must(g => Enum.TryParse<GoalType>(g, true, out _))
				.WithMessage("Nieprawidłowy cel (dozwolone: LoseWeight, Maintain, GainWeight)");

			RuleFor(x => x.TargetWeightKg)
				.NotNull().WithMessage("Docelowa masa ciała jest wymagana")
				.InclusiveBetween(30, 300).WithMessage("Docelowa waga musi być z przedziału 30–300 kg");

			RuleFor(x => x.MealPlan)
				.NotNull().WithMessage("Wybierz plan posiłków")
				.Must(p => p != null && p.Count == 6).WithMessage("Plan posiłków musi zawierać dokładnie 6 pozycji (true/false).")
				.Must(p => p != null && p.Any(x => x)).WithMessage("Wybierz przynajmniej jeden posiłek.");

			RuleFor(x => x.WeeklyGoalChangeKg)
				.NotNull().WithMessage("Tygodniowa zmiana masy jest wymagana.")
				.InclusiveBetween(-1.0f, 1.0f).WithMessage("Tygodniowa zmiana masy musi być z przedziału od -1 do 1 kg.")
				.Must((model, value) =>
			{
				if (model.Goal is null || value is null)
					return false;

				return model.Goal switch
				{
					"LoseWeight" => value < 0,
					"Maintain" => value == 0,
					"GainWeight" => value > 0,
					_ => false
				};
			})
			.WithMessage(x => x.Goal switch
			{
				"LoseWeight" => "Dla celu redukcji masa musi się zmniejszać (wartość ujemna).",
				"Maintain" => "Dla celu utrzymania masa nie może się zmieniać (wartość musi być 0).",
				"GainWeight" => "Dla celu przyrostu masa musi się zwiększać (wartość dodatnia).",
				_ => "Nieprawidłowy cel."
			});

		}
	}
}
