// Plik NutritionCalculationService.cs - serwis domenowy do kalkulacji dziennego zapotrzebowania żywieniowego.
// Odpowiada za obliczanie BMR, TDEE, makroskładników oraz zapotrzebowania na wodę na podstawie profilu użytkownika.

using CalorieTracker.Domain.Entities;
using CalorieTracker.Domain.Enums;
using CalorieTracker.Domain.ValueObjects;

namespace CalorieTracker.Domain.Services
{
	/// <summary>
	/// Serwis domenowy do precyzyjnej kalkulacji dziennego zapotrzebowania żywieniowego.
	/// Implementuje najnowsze formuły i wytyczne żywieniowe dla obliczania BMR, TDEE i makroskładników.
	/// Zawiera czystą logikę biznesową bez zależności zewnętrznych.
	/// </summary>
	public class NutritionCalculationService
	{
		/// <summary>
		/// Oblicza kompletne dzienne zapotrzebowanie żywieniowe użytkownika.
		/// Używa formuły Mifflin-St Jeor do BMR oraz aktualne wytyczne żywieniowe dla makroskładników.
		/// </summary>
		/// <param name="userProfile">Profil użytkownika zawierający wszystkie wymagane dane.</param>
		/// <returns>Obiekt <see cref="DailyNutritionRequirements"/> z kompletnymi wyliczeniami.</returns>
		/// <exception cref="ArgumentException">Gdy profil użytkownika jest niekompletny.</exception>
		public DailyNutritionRequirements CalculateDailyRequirements(UserProfile userProfile)
		{
			ValidateUserProfile(userProfile);

			var bmr = CalculateBMR(userProfile);
			var tdee = CalculateTDEE(bmr, userProfile.ActivityLevel!.Value);
			var targetCalories = AdjustCaloriesForGoal(tdee, userProfile.Goal!.Value, userProfile.WeeklyGoalChangeKg!.Value);

			var proteinRange = CalculateProteinRequirements(userProfile.WeightKg!.Value, userProfile.Goal!.Value);
			var fatRange = CalculateFatRequirements(targetCalories, userProfile.Goal!.Value);
			var carbRange = CalculateCarbohydrateRequirements(targetCalories, proteinRange, fatRange);
			var water = CalculateWaterRequirements(userProfile.WeightKg!.Value, userProfile.ActivityLevel!.Value);

			return new DailyNutritionRequirements(
				calories: targetCalories,
				proteinMinGrams: proteinRange.Min,
				proteinMaxGrams: proteinRange.Max,
				fatMinGrams: fatRange.Min,
				fatMaxGrams: fatRange.Max,
				carbohydratesMinGrams: carbRange.Min,
				carbohydratesMaxGrams: carbRange.Max,
				waterLiters: water,
				bmr: bmr,
				tdee: tdee
			);
		}

		/// <summary>
		/// Oblicza podstawowy wskaźnik metabolizmu (BMR) używając formuły Mifflin-St Jeor.
		/// Najdokładniejsza formuła dla współczesnej populacji.
		/// </summary>
		/// <param name="userProfile">Profil użytkownika z danymi fizycznymi.</param>
		/// <returns>BMR w kilokalorachach na dobę.</returns>
		private float CalculateBMR(UserProfile userProfile)
		{
			var weight = userProfile.WeightKg!.Value;
			var height = userProfile.HeightCm!.Value;
			var age = userProfile.Age!.Value;
			var gender = userProfile.Gender!.Value;

			// Formuła Mifflin-St Jeor
			float bmr;
			if (gender == Gender.Male)
			{
				bmr = 10 * weight + 6.25f * height - 5 * age + 5;
			}
			else // Female
			{
				bmr = 10 * weight + 6.25f * height - 5 * age - 161;
			}

			return (float)Math.Round(bmr, 1);
		}

		/// <summary>
		/// Oblicza całkowity wydatek energetyczny (TDEE) przez pomnożenie BMR przez współczynnik aktywności.
		/// Używa zrewidowanych współczynników według najnowszych badań.
		/// </summary>
		/// <param name="bmr">Podstawowy wskaźnik metabolizmu.</param>
		/// <param name="activityLevel">Poziom aktywności fizycznej użytkownika.</param>
		/// <returns>TDEE w kilokalorachach na dobę.</returns>
		private float CalculateTDEE(float bmr, ActivityLevel activityLevel)
		{
			var activityMultiplier = activityLevel switch
			{
				ActivityLevel.Sedentary => 1.2f,           // Brak lub minimalna aktywność
				ActivityLevel.LightlyActive => 1.375f,     // Lekka aktywność 1-3 dni/tydzień
				ActivityLevel.ModeratelyActive => 1.55f,   // Umiarkowana aktywność 3-5 dni/tydzień
				ActivityLevel.VeryActive => 1.725f,        // Intensywna aktywność 6-7 dni/tydzień
				ActivityLevel.ExtremelyActive => 1.9f,     // Bardzo intensywna aktywność + praca fizyczna
				_ => throw new ArgumentException($"Nieobsługiwany poziom aktywności: {activityLevel}")
			};

			return (float)Math.Round(bmr * activityMultiplier, 1);
		}

		/// <summary>
		/// Dostosowuje kalorie do celu wagowego użytkownika.
		/// Oblicza deficyt/nadwyżkę kalorii na podstawie tygodniowego celu zmiany wagi.
		/// </summary>
		/// <param name="tdee">Całkowity wydatek energetyczny.</param>
		/// <param name="goal">Cel wagowy użytkownika.</param>
		/// <param name="weeklyGoalChangeKg">Tygodniowa zmiana wagi w kg.</param>
		/// <returns>Docelowe dzienne spożycie kalorii.</returns>
		private float AdjustCaloriesForGoal(float tdee, GoalType goal, float weeklyGoalChangeKg)
		{
			// 1 kg tkanki tłuszczowej = 7700 kcal
			var dailyCalorieAdjustment = (weeklyGoalChangeKg * 7700) / 7f;

			var targetCalories = goal switch
			{
				GoalType.LoseWeight => tdee - Math.Abs(dailyCalorieAdjustment),
				GoalType.GainWeight => tdee + Math.Abs(dailyCalorieAdjustment),
				GoalType.Maintain => tdee,
				_ => throw new ArgumentException($"Nieobsługiwany cel: {goal}")
			};

			// Zabezpieczenie przed zbyt drastycznymi dietami
			var minCalories = goal == GoalType.LoseWeight ? Math.Max(1200f, tdee * 0.75f) : 1200f;
			var maxCalories = goal == GoalType.GainWeight ? tdee * 1.5f : tdee * 1.25f;

			return (float)Math.Round(Math.Clamp(targetCalories, minCalories, maxCalories), 1);
		}

		/// <summary>
		/// Oblicza zapotrzebowanie na białko w gramach według aktualnych wytycznych żywieniowych.
		/// Uwzględnia cel wagowy i aktywność fizyczną użytkownika.
		/// </summary>
		/// <param name="weightKg">Masa ciała w kilogramach.</param>
		/// <param name="goal">Cel wagowy użytkownika.</param>
		/// <returns>Przedział zapotrzebowania na białko (min-max).</returns>
		private (float Min, float Max) CalculateProteinRequirements(float weightKg, GoalType goal)
		{
			var (minMultiplier, maxMultiplier) = goal switch
			{
				GoalType.LoseWeight => (1.2f, 1.6f),   // Wyższe białko podczas redukcji
				GoalType.GainWeight => (1.4f, 2.0f),   // Budowa masy mięśniowej
				GoalType.Maintain => (0.8f, 1.2f),     // Standardowe zalecenia
				_ => throw new ArgumentException($"Nieobsługiwany cel: {goal}")
			};

			var minProtein = weightKg * minMultiplier;
			var maxProtein = weightKg * maxMultiplier;

			return (
				Min: (float)Math.Round(minProtein, 1),
				Max: (float)Math.Round(maxProtein, 1)
			);
		}

		/// <summary>
		/// Oblicza zapotrzebowanie na tłuszcze jako procent dziennych kalorii.
		/// Uwzględnia cel żywieniowy dla optymalnej dystrybucji makroskładników.
		/// </summary>
		/// <param name="targetCalories">Docelowe dzienne kalorie.</param>
		/// <param name="goal">Cel wagowy użytkownika.</param>
		/// <returns>Przedział zapotrzebowania na tłuszcze w gramach (min-max).</returns>
		private (float Min, float Max) CalculateFatRequirements(float targetCalories, GoalType goal)
		{
			var (minPercentage, maxPercentage) = goal switch
			{
				GoalType.LoseWeight => (0.20f, 0.30f),   // 20-30% kalorii z tłuszczów
				GoalType.GainWeight => (0.25f, 0.35f),   // Wyżej dla budowy masy
				GoalType.Maintain => (0.20f, 0.35f),     // Szeroki przedział
				_ => throw new ArgumentException($"Nieobsługiwany cel: {goal}")
			};

			// 1 gram tłuszczu = 9 kcal
			var minFat = (targetCalories * minPercentage) / 9f;
			var maxFat = (targetCalories * maxPercentage) / 9f;

			return (
				Min: (float)Math.Round(minFat, 1),
				Max: (float)Math.Round(maxFat, 1)
			);
		}

		/// <summary>
		/// Oblicza zapotrzebowanie na węglowodany jako pozostałość po białku i tłuszczach.
		/// Stosuje metodę "carbs by difference" zalecaną przez dietetyków.
		/// </summary>
		/// <param name="targetCalories">Docelowe dzienne kalorie.</param>
		/// <param name="proteinRange">Przedział białka w gramach.</param>
		/// <param name="fatRange">Przedział tłuszczów w gramach.</param>
		/// <returns>Przedział zapotrzebowania na węglowodany w gramach (min-max).</returns>
		private (float Min, float Max) CalculateCarbohydrateRequirements(
			float targetCalories,
			(float Min, float Max) proteinRange,
			(float Min, float Max) fatRange)
		{
			// 1 gram białka = 4 kcal, 1 gram tłuszczu = 9 kcal, 1 gram węglowodanów = 4 kcal
			var proteinCaloriesMax = proteinRange.Max * 4f;
			var proteinCaloriesMin = proteinRange.Min * 4f;
			var fatCaloriesMax = fatRange.Max * 9f;
			var fatCaloriesMin = fatRange.Min * 9f;

			// Minimum węglowodanów: kalorie - maksymalne białko - maksymalne tłuszcze
			var carbCaloriesMin = targetCalories - proteinCaloriesMax - fatCaloriesMax;
			// Maksimum węglowodanów: kalorie - minimalne białko - minimalne tłuszcze
			var carbCaloriesMax = targetCalories - proteinCaloriesMin - fatCaloriesMin;

			var minCarbs = Math.Max(0f, carbCaloriesMin / 4f);
			var maxCarbs = Math.Max(0f, carbCaloriesMax / 4f);

			return (
				Min: (float)Math.Round(minCarbs, 1),
				Max: (float)Math.Round(maxCarbs, 1)
			);
		}

		/// <summary>
		/// Oblicza dzienne zapotrzebowanie na wodę według wytycznych Institute of Medicine (IOM).
		/// Uwzględnia masę ciała i poziom aktywności fizycznej.
		/// </summary>
		/// <param name="weightKg">Masa ciała w kilogramach.</param>
		/// <param name="activityLevel">Poziom aktywności fizycznej.</param>
		/// <returns>Zapotrzebowanie na wodę w litrach na dobę.</returns>
		private float CalculateWaterRequirements(float weightKg, ActivityLevel activityLevel)
		{
			// Podstawowe zapotrzebowanie: 30ml/kg masy ciała
			var baseWater = weightKg * 0.030f;

			// dodatkowa woda zależna od aktywności
			var activityBonus = activityLevel switch
			{
				ActivityLevel.Sedentary => 0f,
				ActivityLevel.LightlyActive => 0.2f,       
				ActivityLevel.ModeratelyActive => 0.3f,    
				ActivityLevel.VeryActive => 0.5f,          
				ActivityLevel.ExtremelyActive => 0.7f,     
				_ => 0f
			};

			var totalWater = baseWater + activityBonus;

			// Zabezpieczenia: minimum 2L, maksimum 3.5L
			return (float)Math.Round(Math.Clamp(totalWater, 2.0f, 3.5f), 1);
		}

		/// <summary>
		/// Waliduje kompletność profilu użytkownika wymaganą do kalkulacji żywieniowych.
		/// Sprawdza obecność wszystkich wymaganych pól.
		/// </summary>
		/// <param name="userProfile">Profil użytkownika do walidacji.</param>
		/// <exception cref="ArgumentException">Gdy brakuje wymaganych danych.</exception>
		private static void ValidateUserProfile(UserProfile userProfile)
		{
			if (userProfile == null)
				throw new ArgumentException("Profil użytkownika nie może być null");

			var missingFields = new List<string>();

			if (!userProfile.Age.HasValue) missingFields.Add("Wiek");
			if (!userProfile.Gender.HasValue) missingFields.Add("Płeć");
			if (!userProfile.HeightCm.HasValue) missingFields.Add("Wzrost");
			if (!userProfile.WeightKg.HasValue) missingFields.Add("Waga");
			if (!userProfile.ActivityLevel.HasValue) missingFields.Add("Poziom aktywności");
			if (!userProfile.Goal.HasValue) missingFields.Add("Cel");
			if (!userProfile.WeeklyGoalChangeKg.HasValue) missingFields.Add("Tygodniowa zmiana wagi");

			if (missingFields.Any())
			{
				throw new ArgumentException($"Brakuje wymaganych danych w profilu: {string.Join(", ", missingFields)}");
			}
		}
	}
}