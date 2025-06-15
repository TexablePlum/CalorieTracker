// Plik WeightAnalysisService.cs – serwis domenowy do analizy masy ciała użytkownika.
// Odpowiada za obliczanie wskaźnika BMI, zmiany masy ciała oraz postępu względem celu wagowego.
// Wykorzystywany przy tworzeniu i analizie pomiarów masy ciała użytkownika.

using CalorieTracker.Domain.Entities;

namespace CalorieTracker.Domain.Services
{
	/// <summary>
	/// Serwis domenowy do kalkulacji BMI i analizy masy ciała użytkownika.
	/// Zawiera metody do obliczania wskaźnika BMI, zmiany masy oraz różnicy względem celu wagowego.
	/// </summary>
	public class WeightAnalysisService
	{
		/// <summary>
		/// Oblicza wskaźnik masy ciała (BMI) na podstawie masy i wzrostu użytkownika.
		/// Wzór: BMI = masa(kg) / (wzrost(m))²
		/// </summary>
		/// <param name="weightKg">Masa ciała w kilogramach.</param>
		/// <param name="heightCm">Wzrost w centymetrach.</param>
		/// <returns>Wartość BMI (zaokrąglona do jednego miejsca po przecinku).</returns>
		/// <exception cref="ArgumentException">Rzucany, gdy masa lub wzrost są niedodatnie.</exception>
		public float CalculateBMI(float weightKg, float heightCm)
		{
			if (heightCm <= 0 || weightKg <= 0)
				throw new ArgumentException("Wzrost i waga muszą być większe od zera");

			var heightInMeters = heightCm / 100f;
			var bmi = weightKg / (heightInMeters * heightInMeters);

			return (float)Math.Round(bmi, 1); // Zaokrąglenie do 1 miejsca po przecinku
		}

		/// <summary>
		/// Oblicza zmianę masy ciała pomiędzy dwoma pomiarami.
		/// </summary>
		/// <param name="currentWeight">Aktualna masa ciała.</param>
		/// <param name="previousWeight">Poprzednia masa ciała.</param>
		/// <returns>Różnica w kg (dodatnia = przyrost, ujemna = spadek).</returns>
		public float CalculateWeightChange(float currentWeight, float previousWeight)
		{
			var change = currentWeight - previousWeight;
			return (float)Math.Round(change, 1);
		}

		/// <summary>
		/// Oblicza postęp użytkownika względem docelowej masy ciała.
		/// </summary>
		/// <param name="currentWeightKg">Aktualna masa ciała.</param>
		/// <param name="targetWeightKg">Docelowa masa ciała.</param>
		/// <returns>Różnica w kg (dodatnia = nadal trzeba schudnąć, ujemna = przekroczono cel).</returns>
		public float CalculateProgressToGoal(float currentWeightKg, float targetWeightKg)
		{
			var progress = currentWeightKg - targetWeightKg;
			return (float)Math.Round(progress, 1);
		}

		/// <summary>
		/// Uzupełnia pola pochodne w pomiarze masy ciała, takie jak BMI i zmiana masy.
		/// Wartości te są obliczane na podstawie aktualnego pomiaru, profilu użytkownika oraz ewentualnego poprzedniego pomiaru.
		/// </summary>
		/// <param name="measurement">Pomiar masy ciała do uzupełnienia.</param>
		/// <param name="userProfile">Profil użytkownika zawierający dane wzrostu i celu wagowego.</param>
		/// <param name="previousMeasurement">Poprzedni pomiar masy ciała (może być null).</param>
		public void FillCalculatedFields(WeightMeasurement measurement, UserProfile userProfile, WeightMeasurement? previousMeasurement)
		{
			// Oblicza BMI, jeśli wzrost użytkownika jest znany
			if (userProfile.HeightCm.HasValue)
			{
				measurement.BMI = CalculateBMI(measurement.WeightKg, userProfile.HeightCm.Value);
			}

			// Oblicza zmianę masy ciała w porównaniu do poprzedniego pomiaru
			if (previousMeasurement != null)
			{
				measurement.WeightChangeKg = CalculateWeightChange(measurement.WeightKg, previousMeasurement.WeightKg);
			}
			else
			{
				measurement.WeightChangeKg = 0f; // Pierwszy pomiar – brak danych do porównania
			}
		}
	}
}
