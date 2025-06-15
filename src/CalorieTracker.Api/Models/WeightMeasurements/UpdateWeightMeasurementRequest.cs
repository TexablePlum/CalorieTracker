// Plik WeightMeasurements.cs - modele transferu danych (DTO) związane z pomiarami masy ciała.
// Odpowiada za operacje aktualizacji istniejących pomiarów wagi użytkownika w aplikacji.
namespace CalorieTracker.Api.Models.WeightMeasurements
{
	/// <summary>
	/// Model transferu danych (DTO) do aktualizacji istniejącego pomiaru masy ciała.
	/// Zawiera wszystkie możliwe do zaktualizowania dane pomiaru wagi użytkownika.
	/// </summary>
	public class UpdateWeightMeasurementRequest
	{
		/// <summary>
		/// Nowa data wykonania pomiaru wagi.
		/// Format daty zawiera tylko informację o dniu, miesiącu i roku (bez czasu).
		/// </summary>
		public DateOnly MeasurementDate { get; set; }

		/// <summary>
		/// Zaktualizowana wartość pomiaru wagi w kilogramach.
		/// Wartość powinna być dodatnia i realistyczna dla człowieka.
		/// </summary>
		public float WeightKg { get; set; }
	}
}