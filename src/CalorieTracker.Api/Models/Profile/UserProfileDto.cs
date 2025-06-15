// Plik UserProfileDto.cs - model transferu danych (DTO) dla profilu użytkownika.
// Odpowiada za przekazywanie danych profilowych użytkownika między warstwą API a klientem.
namespace CalorieTracker.Api.Models.Profile
{
	/// <summary>
	/// Model transferu danych (DTO) reprezentujący profil użytkownika.
	/// Zawiera podstawowe informacje o użytkowniku oraz szczegóły jego profilu fitness.
	/// </summary>
	public class UserProfileDto
	{
		/// <summary>
		/// Flaga wskazująca, czy profil użytkownika jest kompletny.
		/// Wartość true oznacza, że wszystkie wymagane dane zostały uzupełnione.
		/// </summary>
		public bool IsComplete { get; set; }

		/// <summary>
		/// Adres e-mail użytkownika.
		/// Pobrany z podstawowych danych konta (ApplicationUser).
		/// </summary>
		public string Email { get; set; } = null!;

		/// <summary>
		/// Imię użytkownika.
		/// Pobrane z podstawowych danych konta (ApplicationUser).
		/// </summary>
		public string? FirstName { get; set; }

		/// <summary>
		/// Nazwisko użytkownika.
		/// Pobrane z podstawowych danych konta (ApplicationUser).
		/// </summary>
		public string? LastName { get; set; }

		/// <summary>
		/// Cel fitness użytkownika (np. utrata wagi, przybranie masy).
		/// Pobrane z danych profilowych (UserProfile).
		/// </summary>
		public string? Goal { get; set; }

		/// <summary>
		/// Płeć użytkownika.
		/// Pobrana z danych profilowych (UserProfile).
		/// </summary>
		public string? Gender { get; set; }

		/// <summary>
		/// Wiek użytkownika w latach.
		/// Pobrany z danych profilowych (UserProfile).
		/// </summary>
		public int? Age { get; set; }

		/// <summary>
		/// Wzrost użytkownika w centymetrach.
		/// Pobrany z danych profilowych (UserProfile).
		/// </summary>
		public float? HeightCm { get; set; }

		/// <summary>
		/// Aktualna waga użytkownika w kilogramach.
		/// Pobrana z danych profilowych (UserProfile).
		/// </summary>
		public float? WeightKg { get; set; }

		/// <summary>
		/// Docelowa waga użytkownika w kilogramach.
		/// Pobrana z danych profilowych (UserProfile).
		/// </summary>
		public float? TargetWeightKg { get; set; }

		/// <summary>
		/// Poziom aktywności fizycznej użytkownika.
		/// Pobrany z danych profilowych (UserProfile).
		/// </summary>
		public string? ActivityLevel { get; set; }

		/// <summary>
		/// Tygodniowy cel zmiany wagi w kilogramach.
		/// Pobrany z danych profilowych (UserProfile).
		/// </summary>
		public float? WeeklyGoalChangeKg { get; set; }

		/// <summary>
		/// Plan posiłków użytkownika reprezentowany jako lista flag.
		/// Każdy element listy odpowiada za określoną właściwość planu.
		/// Pobrany z danych profilowych (UserProfile).
		/// </summary>
		public List<bool>? MealPlan { get; set; }
	}
}