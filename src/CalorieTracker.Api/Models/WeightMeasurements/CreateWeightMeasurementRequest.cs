// Plik WeightMeasurements.cs - modele transferu danych (DTO) związane z pomiarami masy ciała.
// Odpowiada za operacje tworzenia i zarządzania pomiarami wagi użytkownika w aplikacji.
namespace CalorieTracker.Api.Models.WeightMeasurements
{
	/// <summary>
	/// Model transferu danych (DTO) do tworzenia nowego pomiaru masy ciała.
	/// Zawiera niezbędne dane do zarejestrowania pomiaru wagi użytkownika w systemie.
	/// </summary>
	public class CreateWeightMeasurementRequest
	{
		/// <summary>
		/// Data wykonania pomiaru wagi.
		/// Format daty zawiera tylko informację o dniu, miesiącu i roku (bez czasu).
		/// </summary>
		public DateOnly MeasurementDate { get; set; }

		/// <summary>
		/// Zmierzona waga użytkownika w kilogramach.
		/// Wartość powinna być dodatnia i realistyczna dla człowieka.
		/// </summary>
		public float WeightKg { get; set; }
	}
}