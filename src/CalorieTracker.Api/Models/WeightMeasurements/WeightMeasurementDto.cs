// Plik WeightMeasurements.cs - modele transferu danych (DTO) związane z pomiarami masy ciała.
// Odpowiada za zwracanie szczegółowych informacji o pomiarach wagi użytkownika przez API.
namespace CalorieTracker.Api.Models.WeightMeasurements
{
	/// <summary>
	/// Model transferu danych (DTO) zawierający pełne informacje o pomiarze masy ciała.
	/// Używany jako odpowiedź API, zawiera zarówno podstawowe dane pomiaru,
	/// jak i dodatkowe obliczone metryki (BMI, progres, etc.).
	/// </summary>
	public class WeightMeasurementDto
	{
		/// <summary>
		/// Unikalny identyfikator pomiaru w systemie.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Data wykonania pomiaru wagi.
		/// Format daty zawiera tylko informację o dniu, miesiącu i roku (bez czasu).
		/// </summary>
		public DateOnly MeasurementDate { get; set; }

		/// <summary>
		/// Zmierzona waga użytkownika w kilogramach.
		/// </summary>
		public float WeightKg { get; set; }

		/// <summary>
		/// Wskaźnik masy ciała (BMI) obliczony na podstawie wagi i wzrostu użytkownika.
		/// </summary>
		public float BMI { get; set; }

		/// <summary>
		/// Zmiana wagi w kilogramach w porównaniu do poprzedniego pomiaru.
		/// Wartość dodatnia oznacza przyrost masy, ujemna - spadek.
		/// </summary>
		public float WeightChangeKg { get; set; }

		/// <summary>
		/// Progres w osiąganiu celu wagowego wyrażony w procentach.
		/// Wartość 100% oznacza osiągnięcie celu.
		/// </summary>
		public float ProgressToGoal { get; set; }

		/// <summary>
		/// Data i czas utworzenia rekordu pomiaru w systemie.
		/// Zawiera pełną informację temporalną (dzień, miesiąc, rok, godzina, minuta, sekunda).
		/// </summary>
		public DateTime CreatedAt { get; set; }
	}
}