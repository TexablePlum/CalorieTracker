// Plik NutritionCalculationServiceTests.cs - testy jednostkowe serwisu kalkulacji żywieniowych.
// Weryfikuje poprawność implementacji algorytmów BMR, TDEE i kalkulacji makroskładników.

using CalorieTracker.Domain.Entities;
using CalorieTracker.Domain.Enums;
using CalorieTracker.Domain.Services;
using Xunit;

namespace CalorieTracker.Tests.Services
{
	/// <summary>
	/// Testy jednostkowe dla serwisu kalkulacji żywieniowych.
	/// Weryfikuje poprawność implementacji formuł BMR, TDEE i dystrybucji makroskładników.
	/// Zapewnia zgodność z wytycznymi WHO/FAO i IOM dla różnych profili użytkowników.
	/// </summary>
	public class NutritionCalculationServiceTests
	{
		/// <summary>
		/// Instancja testowanego serwisu kalkulacji żywieniowych.
		/// </summary>
		private readonly NutritionCalculationService _service;

		/// <summary>
		/// Inicjalizuje nową instancję testów z czystym serwisem kalkulacji.
		/// </summary>
		public NutritionCalculationServiceTests()
		{
			_service = new NutritionCalculationService();
		}

		#region BMR Calculation Tests

		/// <summary>
		/// Weryfikuje poprawność kalkulacji BMR dla mężczyzn używając formuły Mifflin-St Jeor.
		/// Test sprawdza przypadek referencyjny z dobrze znanymi parametrami.
		/// </summary>
		/// <remarks>
		/// <para><strong>Testowany przypadek:</strong></para>
		/// <list type="bullet">
		/// <item>Mężczyzna, 30 lat, 180cm, 80kg</item>
		/// <item>Formuła: BMR = 10 * 80 + 6.25 * 180 - 5 * 30 + 5</item>
		/// <item>Oczekiwany wynik: 1930 kcal/dzień</item>
		/// </list>
		/// </remarks>
		[Fact]
		public void CalculateDailyRequirements_MaleProfile_ShouldCalculateCorrectBMR()
		{
			// Arrange - profil mężczyzny 30 lat, 180cm, 80kg
			var profile = CreateUserProfile(
				gender: Gender.Male,
				age: 30,
				heightCm: 180,
				weightKg: 80,
				activityLevel: ActivityLevel.ModeratelyActive,
				goal: GoalType.Maintain,
				weeklyChange: 0f
			);

			// Act - kalkulacja wymagań żywieniowych
			var result = _service.CalculateDailyRequirements(profile);

			// Assert - weryfikacja BMR według formuły Mifflin-St Jeor
			// BMR = 10 * 80 + 6.25 * 180 - 5 * 30 + 5 = 800 + 1125 - 150 + 5 = 1780
			Assert.Equal(1780f, result.BMR);
		}

		/// <summary>
		/// Weryfikuje poprawność kalkulacji BMR dla kobiet używając formuły Mifflin-St Jeor.
		/// Test sprawdza różnicę w formule między płciami (-161 vs +5).
		/// </summary>
		[Fact]
		public void CalculateDailyRequirements_FemaleProfile_ShouldCalculateCorrectBMR()
		{
			// Arrange - profil kobiety 25 lat, 165cm, 65kg
			var profile = CreateUserProfile(
				gender: Gender.Female,
				age: 25,
				heightCm: 165,
				weightKg: 65,
				activityLevel: ActivityLevel.LightlyActive,
				goal: GoalType.LoseWeight,
				weeklyChange: -0.5f
			);

			// Act
			var result = _service.CalculateDailyRequirements(profile);

			// Assert - BMR = 10 * 65 + 6.25 * 165 - 5 * 25 - 161 = 650 + 1031.25 - 125 - 161 = 1395.25
			// Po zaokrągleniu w serwisie: 1395.2
			Assert.Equal(1395.2f, result.BMR, 1); // Tolerancja 1 miejsce po przecinku
		}

		#endregion

		#region TDEE Calculation Tests

		/// <summary>
		/// Weryfikuje poprawność kalkulacji TDEE dla różnych poziomów aktywności.
		/// Test parametryzowany sprawdzający wszystkie poziomy aktywności fizycznej.
		/// </summary>
		/// <param name="activityLevel">Poziom aktywności fizycznej do testowania.</param>
		/// <param name="expectedMultiplier">Oczekiwany współczynnik aktywności.</param>
		[Theory]
		[InlineData(ActivityLevel.Sedentary, 1.2f)]
		[InlineData(ActivityLevel.LightlyActive, 1.375f)]
		[InlineData(ActivityLevel.ModeratelyActive, 1.55f)]
		[InlineData(ActivityLevel.VeryActive, 1.725f)]
		[InlineData(ActivityLevel.ExtremelyActive, 1.9f)]
		public void CalculateDailyRequirements_DifferentActivityLevels_ShouldCalculateCorrectTDEE(
			ActivityLevel activityLevel, float expectedMultiplier)
		{
			// Arrange - standardowy profil z różnymi poziomami aktywności
			var profile = CreateUserProfile(
				gender: Gender.Male,
				age: 30,
				heightCm: 180,
				weightKg: 80,
				activityLevel: activityLevel,
				goal: GoalType.Maintain,
				weeklyChange: 0f
			);

			// Act
			var result = _service.CalculateDailyRequirements(profile);

			// Assert - TDEE = BMR * współczynnik aktywności
			var expectedTDEE = result.BMR * expectedMultiplier;
			Assert.Equal(expectedTDEE, result.TDEE, 1); // Tolerancja 1 miejsce po przecinku
		}

		#endregion

		#region Goal Adjustment Tests

		/// <summary>
		/// Weryfikuje poprawność dostosowania kalorii do celu utraty wagi.
		/// Test sprawdza kalkulację deficytu kalorycznego na podstawie tygodniowej zmiany wagi.
		/// </summary>
		[Fact]
		public void CalculateDailyRequirements_LoseWeightGoal_ShouldCreateCaloricDeficit()
		{
			// Arrange - profil z celem utraty 0.5kg/tydzień
			var profile = CreateUserProfile(
				gender: Gender.Female,
				age: 30,
				heightCm: 165,
				weightKg: 70,
				activityLevel: ActivityLevel.ModeratelyActive,
				goal: GoalType.LoseWeight,
				weeklyChange: -0.5f // Utrata 0.5kg/tydzień
			);

			// Act
			var result = _service.CalculateDailyRequirements(profile);

			// Assert - kalorie powinny być niższe niż TDEE
			// Deficyt = 0.5kg * 7700 kcal/kg / 7 dni = 550 kcal/dzień
			var expectedDeficit = 0.5f * 7700f / 7f; // ~550 kcal
			var expectedCalories = result.TDEE - expectedDeficit;

			Assert.True(result.Calories < result.TDEE, "Kalorie powinny być niższe niż TDEE dla utraty wagi");
			Assert.Equal(expectedCalories, result.Calories, 0); // Tolerancja 10 kcal - używamy precision 0 dla większej tolerancji
		}

		/// <summary>
		/// Weryfikuje poprawność dostosowania kalorii do celu przyrostu wagi.
		/// Test sprawdza kalkulację nadwyżki kalorycznej.
		/// </summary>
		[Fact]
		public void CalculateDailyRequirements_GainWeightGoal_ShouldCreateCaloricSurplus()
		{
			// Arrange - profil z celem przyrostu 0.3kg/tydzień
			var profile = CreateUserProfile(
				gender: Gender.Male,
				age: 25,
				heightCm: 185,
				weightKg: 75,
				activityLevel: ActivityLevel.VeryActive,
				goal: GoalType.GainWeight,
				weeklyChange: 0.3f // Przyrost 0.3kg/tydzień
			);

			// Act
			var result = _service.CalculateDailyRequirements(profile);

			// Assert - kalorie powinny być wyższe niż TDEE
			Assert.True(result.Calories > result.TDEE, "Kalorie powinny być wyższe niż TDEE dla przyrostu wagi");
		}

		/// <summary>
		/// Weryfikuje czy cel utrzymania wagi nie zmienia TDEE.
		/// </summary>
		[Fact]
		public void CalculateDailyRequirements_MaintainGoal_ShouldEqualTDEE()
		{
			// Arrange
			var profile = CreateUserProfile(
				gender: Gender.Male,
				age: 35,
				heightCm: 175,
				weightKg: 80,
				activityLevel: ActivityLevel.ModeratelyActive,
				goal: GoalType.Maintain,
				weeklyChange: 0f
			);

			// Act
			var result = _service.CalculateDailyRequirements(profile);

			// Assert
			Assert.Equal(result.TDEE, result.Calories);
		}

		#endregion

		#region Macronutrient Distribution Tests

		/// <summary>
		/// Weryfikuje poprawność kalkulacji zapotrzebowania na białko dla różnych celów.
		/// Test parametryzowany sprawdzający mnożniki białka dla każdego celu wagowego.
		/// </summary>
		/// <param name="goal">Cel wagowy użytkownika.</param>
		/// <param name="expectedMinMultiplier">Oczekiwany minimalny mnożnik białka (g/kg).</param>
		/// <param name="expectedMaxMultiplier">Oczekiwany maksymalny mnożnik białka (g/kg).</param>
		[Theory]
		[InlineData(GoalType.LoseWeight, 1.2f, 1.6f)]
		[InlineData(GoalType.GainWeight, 1.4f, 2.0f)]
		[InlineData(GoalType.Maintain, 0.8f, 1.2f)]
		public void CalculateDailyRequirements_DifferentGoals_ShouldCalculateCorrectProtein(
			GoalType goal, float expectedMinMultiplier, float expectedMaxMultiplier)
		{
			// Arrange
			var profile = CreateUserProfile(
				gender: Gender.Male,
				age: 30,
				heightCm: 180,
				weightKg: 80,
				activityLevel: ActivityLevel.ModeratelyActive,
				goal: goal,
				weeklyChange: goal == GoalType.LoseWeight ? -0.5f : goal == GoalType.GainWeight ? 0.5f : 0f
			);

			// Act
			var result = _service.CalculateDailyRequirements(profile);

			// Assert - sprawdzenie przedziału białka
			var expectedMinProtein = 80f * expectedMinMultiplier; // 80kg * mnożnik
			var expectedMaxProtein = 80f * expectedMaxMultiplier;

			Assert.Equal(expectedMinProtein, result.ProteinMinGrams, 1);
			Assert.Equal(expectedMaxProtein, result.ProteinMaxGrams, 1);
		}

		/// <summary>
		/// Weryfikuje czy suma kalorii z makroskładników odpowiada docelowym kaloriom.
		/// Test sprawdza bilans energetyczny (4 kcal/g białka, 4 kcal/g węgli, 9 kcal/g tłuszczu).
		/// </summary>
		[Fact]
		public void CalculateDailyRequirements_MacronutrientDistribution_ShouldSumToTargetCalories()
		{
			// Arrange
			var profile = CreateUserProfile(
				gender: Gender.Female,
				age: 28,
				heightCm: 170,
				weightKg: 65,
				activityLevel: ActivityLevel.ModeratelyActive,
				goal: GoalType.LoseWeight,
				weeklyChange: -0.5f
			);

			// Act
			var result = _service.CalculateDailyRequirements(profile);

			// Assert - sprawdzenie bilansu energetycznego (używamy średnich wartości)
			var proteinCalories = result.ProteinAverageGrams * 4f;
			var fatCalories = result.FatAverageGrams * 9f;
			var carbCalories = result.CarbohydratesAverageGrams * 4f;
			var totalMacroCalories = proteinCalories + fatCalories + carbCalories;

			// Tolerancja 50 kcal z uwagi na zaokrąglenia i przedziały
			var tolerance = 50f;
			Assert.True(Math.Abs(result.Calories - totalMacroCalories) <= tolerance,
				$"Różnica między kaloriami ({result.Calories}) a sumą makro ({totalMacroCalories}) przekracza tolerancję {tolerance} kcal");
		}

		#endregion

		#region Water Requirements Tests

		/// <summary>
		/// Weryfikuje poprawność kalkulacji zapotrzebowania na wodę.
		/// Test sprawdza podstawową formułę (30ml/kg) plus bonus za aktywność.
		/// </summary>
		[Theory]
		[InlineData(60f, ActivityLevel.Sedentary, 2.0f)] // 60kg * 0.030 = 1.8L → min 2.0L
		[InlineData(80f, ActivityLevel.VeryActive, 2.9f)] // 80kg * 0.030 + 0.5L bonus = 2.9L
		[InlineData(100f, ActivityLevel.ExtremelyActive, 3.5f)] // 100kg * 0.030 + 0.7L = 3.7L → max 3.5L
		public void CalculateDailyRequirements_WaterCalculation_ShouldFollowIOMGuidelines(
			float weightKg, ActivityLevel activityLevel, float expectedWaterLiters)
		{
			// Arrange
			var profile = CreateUserProfile(
				gender: Gender.Male,
				age: 30,
				heightCm: 180,
				weightKg: weightKg,
				activityLevel: activityLevel,
				goal: GoalType.Maintain,
				weeklyChange: 0f
			);

			// Act
			var result = _service.CalculateDailyRequirements(profile);

			// Assert
			Assert.Equal(expectedWaterLiters, result.WaterLiters, 1); // 1 miejsce po przecinku
		}

		#endregion

		#region Validation Tests

		/// <summary>
		/// Weryfikuje czy serwis rzuca wyjątek dla profilu z brakującymi danymi.
		/// Test sprawdza walidację wymaganych pól profilu użytkownika.
		/// </summary>
		[Fact]
		public void CalculateDailyRequirements_IncompleteProfile_ShouldThrowArgumentException()
		{
			// Arrange - profil z brakującym wiekiem
			var profile = CreateUserProfile(
				gender: Gender.Male,
				age: null, // Brakujący wiek
				heightCm: 180,
				weightKg: 80,
				activityLevel: ActivityLevel.ModeratelyActive,
				goal: GoalType.Maintain,
				weeklyChange: 0f
			);

			// Act & Assert
			var exception = Assert.Throws<ArgumentException>(() =>
				_service.CalculateDailyRequirements(profile));

			Assert.Contains("Wiek", exception.Message);
		}

		/// <summary>
		/// Weryfikuje czy serwis rzuca wyjątek dla null profilu.
		/// </summary>
		[Fact]
		public void CalculateDailyRequirements_NullProfile_ShouldThrowArgumentException()
		{
			// Act & Assert
			var exception = Assert.Throws<ArgumentException>(() =>
				_service.CalculateDailyRequirements(null!));

			Assert.Contains("null", exception.Message);
		}

		#endregion

		#region Safety Limits Tests

		/// <summary>
		/// Weryfikuje czy serwis stosuje zabezpieczenia dla ekstremalnie niskich kalorii.
		/// Test sprawdza minimum 1200 kcal dla bezpieczeństwa zdrowotnego.
		/// </summary>
		[Fact]
		public void CalculateDailyRequirements_ExtremeLowCalories_ShouldApplySafetyLimits()
		{
			// Arrange - małą osobę z bardzo agresywnym celem redukcji
			var profile = CreateUserProfile(
				gender: Gender.Female,
				age: 25,
				heightCm: 150,
				weightKg: 45,
				activityLevel: ActivityLevel.Sedentary,
				goal: GoalType.LoseWeight,
				weeklyChange: -1.0f // Bardzo agresywna redukcja
			);

			// Act
			var result = _service.CalculateDailyRequirements(profile);

			// Assert - minimum 1200 kcal dla bezpieczeństwa
			Assert.True(result.Calories >= 1200f,
				"Kalorie nie powinny spadać poniżej 1200 dla bezpieczeństwa zdrowotnego");
		}

		/// <summary>
		/// Weryfikuje czy zapotrzebowanie na wodę mieści się w bezpiecznych granicach.
		/// Test sprawdza limity 2-4 litry na dobę.
		/// </summary>
		[Fact]
		public void CalculateDailyRequirements_WaterRequirements_ShouldStayWithinSafeLimits()
		{
			// Arrange - ekstremalnie duża osoba
			var profile = CreateUserProfile(
				gender: Gender.Male,
				age: 30,
				heightCm: 200,
				weightKg: 150,
				activityLevel: ActivityLevel.ExtremelyActive,
				goal: GoalType.GainWeight,
				weeklyChange: 0.5f
			);

			// Act
			var result = _service.CalculateDailyRequirements(profile);

			// Assert - woda w granicach 2-4L
			Assert.InRange(result.WaterLiters, 2.0f, 4.0f);
		}

		#endregion

		#region Helper Methods

		/// <summary>
		/// Factory method tworząca profil użytkownika do celów testowych.
		/// Umożliwia łatwe tworzenie różnych scenariuszy testowych z kontrolowanymi parametrami.
		/// </summary>
		/// <param name="gender">Płeć użytkownika.</param>
		/// <param name="age">Wiek użytkownika (nullable dla testów walidacji).</param>
		/// <param name="heightCm">Wzrost w centymetrach (nullable dla testów walidacji).</param>
		/// <param name="weightKg">Waga w kilogramach (nullable dla testów walidacji).</param>
		/// <param name="activityLevel">Poziom aktywności fizycznej (nullable dla testów walidacji).</param>
		/// <param name="goal">Cel wagowy użytkownika (nullable dla testów walidacji).</param>
		/// <param name="weeklyChange">Tygodniowa zmiana wagi w kg (nullable dla testów walidacji).</param>
		/// <returns>Skonfigurowany profil użytkownika.</returns>
		private UserProfile CreateUserProfile(
			Gender? gender = null,
			int? age = null,
			float? heightCm = null,
			float? weightKg = null,
			ActivityLevel? activityLevel = null,
			GoalType? goal = null,
			float? weeklyChange = null)
		{
			return new UserProfile
			{
				Id = Guid.NewGuid(),
				UserId = "test-user-123",
				Gender = gender,
				Age = age,
				HeightCm = heightCm,
				WeightKg = weightKg,
				TargetWeightKg = weightKg + (weeklyChange * 10), // Domyślny cel: waga +/- 10 tygodni
				ActivityLevel = activityLevel,
				Goal = goal,
				WeeklyGoalChangeKg = weeklyChange,
				MealPlan = [true, true, true, true, true, true] // Wszystkie posiłki
			};
		}

		#endregion
	}
}