// Plik UserProfile.cs - definicja encji profilu użytkownika.
// Reprezentuje szczegółowe dane profilowe użytkownika, w tym cele, dane fizyczne i preferencje żywieniowe.

using CalorieTracker.Domain.Enums;

namespace CalorieTracker.Domain.Entities
{
	/// &lt;summary>
	/// Encja reprezentująca profil użytkownika w systemie.
	/// Zawiera dane dotyczące celów żywieniowych, danych fizycznych oraz preferencji dotyczących posiłków.
	/// &lt;/summary>
	public class UserProfile
	{
		/// &lt;summary>
		/// Unikalny identyfikator GUID profilu użytkownika.
		/// &lt;/summary>
		public Guid Id { get; set; } = Guid.NewGuid();

		// Podstawowe dane użytkownika

		/// <summary>
		/// Cel użytkownika (np. utrata wagi, utrzymanie wagi, przyrost masy), zdefiniowany przez <see cref="GoalType"/>.
		/// Opcjonalny.
		/// </summary>
		public GoalType? Goal { get; set; }

		/// <summary>
		/// Płeć użytkownika, zdefiniowana przez <see cref="Gender"/>.
		/// Opcjonalny.
		/// </summary>
		public Gender? Gender { get; set; }

		/// <summary>
		/// Wiek użytkownika w latach.
		/// Opcjonalny.
		/// </summary>
		public int? Age { get; set; }

		/// <summary>
		/// Wzrost użytkownika w centymetrach.
		/// Opcjonalny.
		/// </summary>
		public float? HeightCm { get; set; }

		/// <summary>
		/// Aktualna waga użytkownika w kilogramach.
		/// Opcjonalny.
		/// </summary>
		public float? WeightKg { get; set; }

		/// <summary>
		/// Docelowa waga użytkownika w kilogramach.
		/// Opcjonalny.
		/// </summary>
		public float? TargetWeightKg { get; set; }

		/// <summary>
		/// Poziom aktywności fizycznej użytkownika, zdefiniowany przez <see cref="ActivityLevel"/>.
		/// Opcjonalny.
		/// </summary>
		public ActivityLevel? ActivityLevel { get; set; }

		/// <summary>
		/// Docelowa tygodniowa zmiana wagi użytkownika w kilogramach (np. -0.5 kg dla utraty wagi).
		/// Opcjonalny.
		/// </summary>
		public float? WeeklyGoalChangeKg { get; set; }

		/// <summary>
		/// Tablica wartości logicznych określająca, które posiłki są uwzględnione w planie żywieniowym użytkownika.
		/// Indeksy odpowiadają: 0-śniadanie, 1-II śniadanie, 2-lunch, 3-obiad, 4-przekąska, 5-kolacja.
		/// Domyślnie wszystkie ustawione na `false`.
		/// </summary>
		public bool[] MealPlan { get; set; } = new bool[6];

		// Relacja 1:1 z User

		/// <summary>
		/// Identyfikator użytkownika, do którego przypisany jest ten profil.
		/// Pole wymagane.
		/// </summary>
		public string UserId { get; set; } = null!;

		/// <summary>
		/// Właściwość nawigacyjna do powiązanego użytkownika <see cref="ApplicationUser"/>.
		/// </summary>
		public ApplicationUser User { get; set; } = null!;
	}
}