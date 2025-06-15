// Plik UpdateUserProfileRequest.cs - model żądania aktualizacji profilu użytkownika.
// Odpowiada za przechwytywanie danych do aktualizacji informacji podstawowych i preferencji użytkownika.

namespace CalorieTracker.Api.Models.Profile
{
	/// <summary>
	/// Model żądania aktualizacji profilu użytkownika.
	/// Zawiera zarówno podstawowe dane użytkownika jak i szczegóły dotyczące celów zdrowotnych i preferencji.
	/// </summary>
	public class UpdateUserProfileRequest
	{
		// Dane z ApplicationUser
		/// <summary>
		/// Imię użytkownika (opcjonalne).
		/// </summary>
		public string? FirstName { get; set; }

		/// <summary>
		/// Nazwisko użytkownika (opcjonalne).
		/// </summary>
		public string? LastName { get; set; }

		// Dane z UserProfile
		/// <summary>
		/// Cel zdrowotny użytkownika (np. utrata wagi, budowa masy) (opcjonalne).
		/// </summary>
		public string? Goal { get; set; }

		/// <summary>
		/// Płeć użytkownika (opcjonalne).
		/// </summary>
		public string? Gender { get; set; }

		/// <summary>
		/// Wiek użytkownika w latach (opcjonalne).
		/// </summary>
		public int? Age { get; set; }

		/// <summary>
		/// Wzrost użytkownika w centymetrach (opcjonalne).
		/// </summary>
		public float? HeightCm { get; set; }

		/// <summary>
		/// Aktualna waga użytkownika w kilogramach (opcjonalne).
		/// </summary>
		public float? WeightKg { get; set; }

		/// <summary>
		/// Docelowa waga użytkownika w kilogramach (opcjonalne).
		/// </summary>
		public float? TargetWeightKg { get; set; }

		/// <summary>
		/// Poziom aktywności fizycznej użytkownika (opcjonalne).
		/// </summary>
		public string? ActivityLevel { get; set; }

		/// <summary>
		/// Tygodniowy cel zmiany wagi w kilogramach (opcjonalne).
		/// Wartość dodatnia - przyrost masy, ujemna - redukcja.
		/// </summary>
		public float? WeeklyGoalChangeKg { get; set; }

		/// <summary>
		/// Plan posiłków użytkownika jako lista flag (opcjonalne).
		/// Określa preferencje dotyczące rozkładu posiłków w ciągu dnia.
		/// </summary>
		public List<bool>? MealPlan { get; set; }
	}
}