using CalorieTracker.Domain.Entities;
using CalorieTracker.Domain.Enums;
using CalorieTracker.Domain.Services;
using Xunit;

namespace CalorieTracker.Tests.Domain.Services
{
	public class WeightAnalysisServiceTests
	{
		private readonly WeightAnalysisService _service;

		public WeightAnalysisServiceTests()
		{
			_service = new WeightAnalysisService();
		}

		[Theory]
		[InlineData(70, 175, 22.9f)]     // Normalny BMI
		[InlineData(60, 160, 23.4f)]     // Normalny BMI  
		[InlineData(90, 180, 27.8f)]     // Nadwaga
		[InlineData(100, 170, 34.6f)]    // Otyłość
		[InlineData(45, 165, 16.5f)]     // Niedowaga
		public void CalculateBMI_ShouldCalculateCorrectly(float weightKg, float heightCm, float expectedBMI)
		{
			// Act
			var result = _service.CalculateBMI(weightKg, heightCm);

			// Assert
			Assert.Equal(expectedBMI, result, 1); // Dokładność do 1 miejsca po przecinku
		}

		[Theory]
		[InlineData(0, 175)]    // Waga = 0
		[InlineData(70, 0)]     // Wzrost = 0
		[InlineData(-10, 175)]  // Ujemna waga
		[InlineData(70, -160)]  // Ujemny wzrost
		public void CalculateBMI_WithInvalidInput_ShouldThrowException(float weightKg, float heightCm)
		{
			// Act & Assert
			Assert.Throws<ArgumentException>(() => _service.CalculateBMI(weightKg, heightCm));
		}

		[Theory]
		[InlineData(76.2f, 75.5f, 0.7f)]    // Przyrost: 76.2 - 75.5 = +0.7
		[InlineData(75.5f, 76.2f, -0.7f)]   // Spadek: 75.5 - 76.2 = -0.7
		[InlineData(70.0f, 70.0f, 0.0f)]    // Bez zmiany: 70.0 - 70.0 = 0.0
		[InlineData(70.1f, 68.7f, 1.4f)]    // Większy przyrost: 70.1 - 68.7 = +1.4
		[InlineData(78.9f, 82.3f, -3.4f)]   // Większy spadek: 78.9 - 82.3 = -3.4
		public void CalculateWeightChange_ShouldCalculateCorrectly(float current, float previous, float expectedChange)
		{
			// Act
			var result = _service.CalculateWeightChange(current, previous);

			// Assert
			Assert.Equal(expectedChange, result, 1);
		}

		[Theory]
		[InlineData(75.5f, 70.0f, 5.5f)]    // Powyżej celu (trzeba schudnąć)
		[InlineData(68.2f, 70.0f, -1.8f)]   // Poniżej celu (przekroczono cel)
		[InlineData(70.0f, 70.0f, 0.0f)]    // Dokładnie w celu
		[InlineData(85.7f, 80.0f, 5.7f)]    // Daleko od celu
		public void CalculateProgressToGoal_ShouldCalculateCorrectly(float current, float target, float expectedProgress)
		{
			// Act
			var result = _service.CalculateProgressToGoal(current, target);

			// Assert
			Assert.Equal(expectedProgress, result, 1);
		}

		[Fact]
		public void FillCalculatedFields_WithValidData_ShouldFillAllFields()
		{
			// Arrange
			var userProfile = CreateUserProfile(heightCm: 175, targetWeight: 70);
			var previousMeasurement = CreateMeasurement(weightKg: 73.5f);
			var currentMeasurement = CreateMeasurement(weightKg: 72.8f);

			// Act
			_service.FillCalculatedFields(currentMeasurement, userProfile, previousMeasurement);

			// Assert
			Assert.Equal(23.8f, currentMeasurement.BMI, 1);          // BMI: 72.8 / (1.75^2) = 23.8
			Assert.Equal(-0.7f, currentMeasurement.WeightChangeKg, 1); // Zmiana: 72.8 - 73.5 = -0.7
		}

		[Fact]
		public void FillCalculatedFields_WithNoPreviousMeasurement_ShouldSetZeroChange()
		{
			// Arrange
			var userProfile = CreateUserProfile(heightCm: 170, targetWeight: 65);
			var currentMeasurement = CreateMeasurement(weightKg: 68.0f);

			// Act
			_service.FillCalculatedFields(currentMeasurement, userProfile, previousMeasurement: null);

			// Assert
			Assert.Equal(23.5f, currentMeasurement.BMI, 1);          // BMI: 68 / (1.7^2) = 23.5
			Assert.Equal(0.0f, currentMeasurement.WeightChangeKg);   // Pierwszy pomiar = 0 zmiany
		}

		[Fact]
		public void FillCalculatedFields_WithNoHeight_ShouldNotCalculateBMI()
		{
			// Arrange
			var userProfile = CreateUserProfile(heightCm: null, targetWeight: 70);
			var previousMeasurement = CreateMeasurement(weightKg: 73.0f);
			var currentMeasurement = CreateMeasurement(weightKg: 72.0f);

			// Act
			_service.FillCalculatedFields(currentMeasurement, userProfile, previousMeasurement);

			// Assert
			Assert.Equal(0f, currentMeasurement.BMI);               // BMI nie obliczone (brak wzrostu)
			Assert.Equal(-1.0f, currentMeasurement.WeightChangeKg, 1); // Zmiana nadal obliczona
		}

		[Theory]
		[InlineData(72.8, 175, 23.8f)]  // Normalny case
		[InlineData(50.0, 150, 22.2f)]  // Niska waga/wzrost
		[InlineData(120, 200, 30.0f)]   // Wysoka waga/wzrost
		public void CalculateBMI_EdgeCases_ShouldHandleCorrectly(float weight, float height, float expectedBMI)
		{
			// Act
			var result = _service.CalculateBMI(weight, height);

			// Assert
			Assert.Equal(expectedBMI, result, 1);
		}

		[Fact]
		public void CalculateWeightChange_WithExtremeValues_ShouldHandleCorrectly()
		{
			// Arrange
			float currentWeight = 150.7f;
			float previousWeight = 45.3f;

			// Act
			var result = _service.CalculateWeightChange(currentWeight, previousWeight);

			// Assert
			Assert.Equal(105.4f, result, 1); // Extreme ale możliwy przypadek
		}

		// Helper methods
		private UserProfile CreateUserProfile(float? heightCm, float? targetWeight)
		{
			return new UserProfile
			{
				Id = Guid.NewGuid(),
				UserId = "test-user",
				HeightCm = heightCm,
				TargetWeightKg = targetWeight,
				Gender = Gender.Male,
				Age = 30,
				ActivityLevel = ActivityLevel.ModeratelyActive,
				Goal = GoalType.LoseWeight
			};
		}

		private WeightMeasurement CreateMeasurement(float weightKg)
		{
			return new WeightMeasurement
			{
				Id = Guid.NewGuid(),
				UserId = "test-user",
				MeasurementDate = DateOnly.FromDateTime(DateTime.Today),
				WeightKg = weightKg,
				CreatedAt = DateTime.UtcNow,
				UpdatedAt = DateTime.UtcNow
			};
		}
	}
}