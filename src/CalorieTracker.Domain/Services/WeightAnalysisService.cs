using CalorieTracker.Domain.Entities;

namespace CalorieTracker.Domain.Services
{
	/// <summary>
	/// Serwis domenowy do kalkulacji BMI i analizy wagi
	/// </summary>
	public class WeightAnalysisService
	{
		/// <summary>
		/// Oblicza BMI na podstawie masy ciała i wzrostu
		/// BMI = masa(kg) / (wzrost(m))²
		/// </summary>
		/// <param name="weightKg">Masa ciała w kilogramach</param>
		/// <param name="heightCm">Wzrost w centymetrach</param>
		/// <returns>Wartość BMI</returns>
		public float CalculateBMI(float weightKg, float heightCm)
		{
			if (heightCm <= 0 || weightKg <= 0)
				throw new ArgumentException("Wzrost i waga muszą być większe od zera");

			var heightInMeters = heightCm / 100f;
			var bmi = weightKg / (heightInMeters * heightInMeters);

			return (float)Math.Round(bmi, 1); // Zaokrąglenie do 1 miejsca po przecinku
		}

		/// <summary>
		/// Oblicza zmianę masy ciała między dwoma pomiarami
		/// </summary>
		/// <param name="currentWeight">Aktualna masa ciała</param>
		/// <param name="previousWeight">Poprzednia masa ciała</param>
		/// <returns>Zmiana w kg (dodatnia = przyrost, ujemna = spadek)</returns>
		public float CalculateWeightChange(float currentWeight, float previousWeight)
		{
			var change = currentWeight - previousWeight;
			return (float)Math.Round(change, 1);
		}

		/// <summary>
		/// Oblicza postęp względem celu wagowego
		/// </summary>
		/// <param name="currentWeightKg">Aktualna masa ciała</param>
		/// <param name="targetWeightKg">Docelowa masa ciała</param>
		/// <returns>Różnica w kg (dodatnia = nadal trzeba schudnąć, ujemna = przekroczono cel)</returns>
		public float CalculateProgressToGoal(float currentWeightKg, float targetWeightKg)
		{
			var progress = currentWeightKg - targetWeightKg;
			return (float)Math.Round(progress, 1);
		}

		/// <summary>
		/// Wypełnia kalkulowane pola w pomiarze masy ciała
		/// </summary>
		/// <param name="measurement">Pomiar do wypełnienia</param>
		/// <param name="userProfile">Profil użytkownika z danymi wzrostu i celu</param>
		/// <param name="previousMeasurement">Poprzedni pomiar (może być null)</param>
		public void FillCalculatedFields(WeightMeasurement measurement, UserProfile userProfile, WeightMeasurement? previousMeasurement)
		{
			// Oblicz BMI jeśli jest wzrost w profilu
			if (userProfile.HeightCm.HasValue)
			{
				measurement.BMI = CalculateBMI(measurement.WeightKg, userProfile.HeightCm.Value);
			}

			// Oblicz zmianę masy ciała jeśli jest poprzedni pomiar
			if (previousMeasurement != null)
			{
				measurement.WeightChangeKg = CalculateWeightChange(measurement.WeightKg, previousMeasurement.WeightKg);
			}
			else
			{
				measurement.WeightChangeKg = 0f; // Pierwszy pomiar
			}
		}
	}
}