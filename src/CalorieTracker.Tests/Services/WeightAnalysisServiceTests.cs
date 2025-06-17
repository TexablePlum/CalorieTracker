using CalorieTracker.Domain.Entities;
using CalorieTracker.Domain.Enums;
using CalorieTracker.Domain.Services;
using Xunit;

namespace CalorieTracker.Tests.Services
{
	/// <summary>
	/// Klasa testowa weryfikująca poprawność działania serwisu <see cref="WeightAnalysisService"/>.
	/// Zawiera testy jednostkowe dla wszystkich algorytmów analizy masy ciała użytkowników,
	/// włączając kalkulacje BMI, zmiany wagi oraz postępu względem celów wagowych.
	/// </summary>
	/// <remarks>
	/// <para><strong>Testowane funkcjonalności:</strong></para>
	/// <list type="bullet">
	/// <item>Kalkulacja wskaźnika BMI dla różnych kategorii wagowych</item>
	/// <item>Obliczanie zmian masy ciała między pomiarami</item>
	/// <item>Analiza postępu względem celów wagowych użytkownika</item>
	/// <item>Automatyczne wypełnianie pól kalkulowanych w pomiarach</item>
	/// <item>Obsługa przypadków granicznych i błędnych danych</item>
	/// </list>
	/// <para><strong>Wzorce testowe:</strong> AAA Pattern, Theory/InlineData, Helper Methods</para>
	/// </remarks>
	public class WeightAnalysisServiceTests
	{
		/// <summary>
		/// Instancja testowanego serwisu analizy masy ciała.
		/// Inicjalizowana w konstruktorze klasy testowej bez dodatkowych zależności.
		/// </summary>
		private readonly WeightAnalysisService _service;

		/// <summary>
		/// Inicjalizuje nową instancję klasy testowej.
		/// Tworzy czystą instancję <see cref="WeightAnalysisService"/> dla każdego testu.
		/// </summary>
		public WeightAnalysisServiceTests()
		{
			_service = new WeightAnalysisService();
		}

		/// <summary>
		/// Weryfikuje poprawność kalkulacji wskaźnika BMI dla różnych kategorii wagowych.
		/// Test parametryzowany sprawdzający algorytm BMI = waga(kg) / (wzrost(m))².
		/// </summary>
		/// <param name="weightKg">Masa ciała w kilogramach.</param>
		/// <param name="heightCm">Wzrost w centymetrach.</param>
		/// <param name="expectedBMI">Oczekiwana wartość BMI zaokrąglona do 1 miejsca po przecinku.</param>
		/// <remarks>
		/// <para><strong>Testowane kategorie BMI:</strong></para>
		/// <list type="table">
		/// <item>
		/// <term>Niedowaga</term>
		/// <description>BMI &lt; 18.5 (45kg, 165cm = 16.5)</description>
		/// </item>
		/// <item>
		/// <term>Waga normalna</term>
		/// <description>BMI 18.5-24.9 (70kg, 175cm = 22.9)</description>
		/// </item>
		/// <item>
		/// <term>Nadwaga</term>
		/// <description>BMI 25-29.9 (90kg, 180cm = 27.8)</description>
		/// </item>
		/// <item>
		/// <term>Otyłość</term>
		/// <description>BMI ≥ 30 (100kg, 170cm = 34.6)</description>
		/// </item>
		/// </list>
		/// <para><strong>Precyzja:</strong> Wyniki zaokrąglane do 1 miejsca po przecinku zgodnie z wymogami medycznymi.</para>
		/// </remarks>
		[Theory]
		[InlineData(70, 175, 22.9f)]     // Normalny BMI
		[InlineData(60, 160, 23.4f)]     // Normalny BMI  
		[InlineData(90, 180, 27.8f)]     // Nadwaga
		[InlineData(100, 170, 34.6f)]    // Otyłość
		[InlineData(45, 165, 16.5f)]     // Niedowaga
		public void CalculateBMI_ShouldCalculateCorrectly(float weightKg, float heightCm, float expectedBMI)
		{
			// Act - wykonanie kalkulacji BMI
			var result = _service.CalculateBMI(weightKg, heightCm);

			// Assert - weryfikacja z dokładnością do 1 miejsca po przecinku
			Assert.Equal(expectedBMI, result, 1);
		}

		/// <summary>
		/// Weryfikuje obsługę nieprawidłowych danych wejściowych w kalkulacji BMI.
		/// Test parametryzowany sprawdzający walidację biznesową przed wykonaniem obliczeń.
		/// </summary>
		/// <param name="weightKg">Nieprawidłowa masa ciała (zero, ujemna).</param>
		/// <param name="heightCm">Nieprawidłowy wzrost (zero, ujemny).</param>
		/// <remarks>
		/// <para><strong>Testowane przypadki błędnych danych:</strong></para>
		/// <list type="bullet">
		/// <item>Waga równa zero (niemożliwa biologicznie)</item>
		/// <item>Wzrost równy zero (niemożliwy biologicznie)</item>
		/// <item>Ujemna waga (nieprawidłowe dane wejściowe)</item>
		/// <item>Ujemny wzrost (nieprawidłowe dane wejściowe)</item>
		/// </list>
		/// <para><strong>Oczekiwane zachowanie:</strong> Wyrzucenie <see cref="ArgumentException"/> 
		/// z opisowym komunikatem o wymaganych wartościach dodatnich.</para>
		/// </remarks>
		[Theory]
		[InlineData(0, 175)]    // Waga = 0
		[InlineData(70, 0)]     // Wzrost = 0
		[InlineData(-10, 175)]  // Ujemna waga
		[InlineData(70, -160)]  // Ujemny wzrost
		public void CalculateBMI_WithInvalidInput_ShouldThrowException(float weightKg, float heightCm)
		{
			// Act & Assert - weryfikacja wyjątku
			Assert.Throws<ArgumentException>(() => _service.CalculateBMI(weightKg, heightCm));
		}

		/// <summary>
		/// Weryfikuje poprawność kalkulacji zmiany masy ciała między dwoma pomiarami.
		/// Test parametryzowany sprawdzający różnicę: aktualna_waga - poprzednia_waga.
		/// </summary>
		/// <param name="current">Aktualna masa ciała w kg.</param>
		/// <param name="previous">Poprzednia masa ciała w kg.</param>
		/// <param name="expectedChange">Oczekiwana zmiana w kg (dodatnia = przyrost, ujemna = spadek).</param>
		/// <remarks>
		/// <para><strong>Testowane scenariusze zmian:</strong></para>
		/// <list type="table">
		/// <item>
		/// <term>Przyrost niewielki</term>
		/// <description>+0.7kg (76.2 - 75.5)</description>
		/// </item>
		/// <item>
		/// <term>Spadek niewielki</term>
		/// <description>-0.7kg (75.5 - 76.2)</description>
		/// </item>
		/// <item>
		/// <term>Brak zmiany</term>
		/// <description>0.0kg (70.0 - 70.0)</description>
		/// </item>
		/// <item>
		/// <term>Znaczący przyrost</term>
		/// <description>+1.4kg (70.1 - 68.7)</description>
		/// </item>
		/// <item>
		/// <term>Znaczący spadek</term>
		/// <description>-3.4kg (78.9 - 82.3)</description>
		/// </item>
		/// </list>
		/// <para><strong>Zastosowanie:</strong> Analiza trendów wagowych, motywacja użytkownika, 
		/// sygnalizowanie zbyt szybkich zmian.</para>
		/// </remarks>
		[Theory]
		[InlineData(76.2f, 75.5f, 0.7f)]    // Przyrost: 76.2 - 75.5 = +0.7
		[InlineData(75.5f, 76.2f, -0.7f)]   // Spadek: 75.5 - 76.2 = -0.7
		[InlineData(70.0f, 70.0f, 0.0f)]    // Bez zmiany: 70.0 - 70.0 = 0.0
		[InlineData(70.1f, 68.7f, 1.4f)]    // Większy przyrost: 70.1 - 68.7 = +1.4
		[InlineData(78.9f, 82.3f, -3.4f)]   // Większy spadek: 78.9 - 82.3 = -3.4
		public void CalculateWeightChange_ShouldCalculateCorrectly(float current, float previous, float expectedChange)
		{
			// Act - kalkulacja zmiany wagi
			var result = _service.CalculateWeightChange(current, previous);

			// Assert - weryfikacja z dokładnością do 1 miejsca po przecinku
			Assert.Equal(expectedChange, result, 1);
		}

		/// <summary>
		/// Weryfikuje kalkulację postępu użytkownika względem ustalonego celu wagowego.
		/// Test parametryzowany sprawdzający różnicę: aktualna_waga - cel_wagowy.
		/// </summary>
		/// <param name="current">Aktualna masa ciała użytkownika w kg.</param>
		/// <param name="target">Docelowa masa ciała użytkownika w kg.</param>
		/// <param name="expectedProgress">Oczekiwany postęp w kg (dodatni = nad celem, ujemny = poniżej celu).</param>
		/// <remarks>
		/// <para><strong>Interpretacja wyników:</strong></para>
		/// <list type="table">
		/// <item>
		/// <term>Wartość dodatnia</term>
		/// <description>Użytkownik jest powyżej celu wagowego (np. +5.5kg = trzeba schudnąć 5.5kg)</description>
		/// </item>
		/// <item>
		/// <term>Wartość ujemna</term>
		/// <description>Użytkownik przekroczył cel (np. -1.8kg = schudł o 1.8kg więcej niż planował)</description>
		/// </item>
		/// <item>
		/// <term>Wartość zero</term>
		/// <description>Użytkownik osiągnął dokładnie swój cel wagowy</description>
		/// </item>
		/// </list>
		/// <para><strong>Zastosowanie UI:</strong> Paski postępu, motywacyjne komunikaty, 
		/// planowanie kolejnych etapów diety.</para>
		/// </remarks>
		[Theory]
		[InlineData(75.5f, 70.0f, 5.5f)]    // Powyżej celu (trzeba schudnąć)
		[InlineData(68.2f, 70.0f, -1.8f)]   // Poniżej celu (przekroczono cel)
		[InlineData(70.0f, 70.0f, 0.0f)]    // Dokładnie w celu
		[InlineData(85.7f, 80.0f, 5.7f)]    // Daleko od celu
		public void CalculateProgressToGoal_ShouldCalculateCorrectly(float current, float target, float expectedProgress)
		{
			// Act - kalkulacja postępu względem celu
			var result = _service.CalculateProgressToGoal(current, target);

			// Assert - weryfikacja z dokładnością do 1 miejsca po przecinku
			Assert.Equal(expectedProgress, result, 1);
		}

		/// <summary>
		/// Weryfikuje automatyczne wypełnianie pól kalkulowanych w pomiarze wagi przy pełnych danych.
		/// Test integracyjny sprawdzający współpracę wszystkich algorytmów kalkulacyjnych.
		/// </summary>
		/// <remarks>
		/// <para><strong>Scenariusz testowy:</strong></para>
		/// <list type="table">
		/// <item>
		/// <term>Profil użytkownika</term>
		/// <description>Wzrost: 175cm, Cel wagowy: 70kg</description>
		/// </item>
		/// <item>
		/// <term>Poprzedni pomiar</term>
		/// <description>73.5kg</description>
		/// </item>
		/// <item>
		/// <term>Aktualny pomiar</term>
		/// <description>72.8kg</description>
		/// </item>
		/// </list>
		/// <para><strong>Oczekiwane kalkulacje:</strong></para>
		/// <list type="bullet">
		/// <item>BMI: 23.8 (72.8 ÷ 1.75²)</item>
		/// <item>Zmiana wagi: -0.7kg (72.8 - 73.5)</item>
		/// </list>
		/// <para><strong>Logika biznesowa:</strong> Automatyzacja obliczeń zapewnia spójność danych 
		/// i odciąża użytkownika od manualnych kalkulacji.</para>
		/// </remarks>
		[Fact]
		public void FillCalculatedFields_WithValidData_ShouldFillAllFields()
		{
			// Arrange - przygotowanie profilu i pomiarów
			var userProfile = CreateUserProfile(heightCm: 175, targetWeight: 70);
			var previousMeasurement = CreateMeasurement(weightKg: 73.5f);
			var currentMeasurement = CreateMeasurement(weightKg: 72.8f);

			// Act - wypełnienie pól kalkulowanych
			_service.FillCalculatedFields(currentMeasurement, userProfile, previousMeasurement);

			// Assert - weryfikacja obliczeń
			Assert.Equal(23.8f, currentMeasurement.BMI, 1);          // BMI: 72.8 / (1.75^2) = 23.8
			Assert.Equal(-0.7f, currentMeasurement.WeightChangeKg, 1); // Zmiana: 72.8 - 73.5 = -0.7
		}

		/// <summary>
		/// Weryfikuje obsługę pierwszego pomiaru użytkownika (brak poprzedniego pomiaru do porównania).
		/// Test sprawdzający inicjalizację systemu pomiarów dla nowych użytkowników.
		/// </summary>
		/// <remarks>
		/// <para><strong>Scenariusz testowy:</strong></para>
		/// <list type="bullet">
		/// <item>Nowy użytkownik wykonuje pierwszy pomiar wagi</item>
		/// <item>Brak poprzedniego pomiaru do porównania (previousMeasurement = null)</item>
		/// <item>Profil zawiera wzrost, więc BMI może być obliczone</item>
		/// </list>
		/// <para><strong>Oczekiwane zachowanie:</strong></para>
		/// <list type="bullet">
		/// <item>BMI obliczone normalnie na podstawie wzrostu z profilu</item>
		/// <item>Zmiana wagi ustawiona na 0.0kg (punkt odniesienia)</item>
		/// </list>
		/// <para><strong>Znaczenie biznesowe:</strong> Pierwsze pomiary stanowią baseline 
		/// dla przyszłych analiz trendów wagowych.</para>
		/// </remarks>
		[Fact]
		public void FillCalculatedFields_WithNoPreviousMeasurement_ShouldSetZeroChange()
		{
			// Arrange - pierwszy pomiar użytkownika
			var userProfile = CreateUserProfile(heightCm: 170, targetWeight: 65);
			var currentMeasurement = CreateMeasurement(weightKg: 68.0f);

			// Act - wypełnienie pól bez poprzedniego pomiaru
			_service.FillCalculatedFields(currentMeasurement, userProfile, previousMeasurement: null);

			// Assert - weryfikacja inicjalizacji
			Assert.Equal(23.5f, currentMeasurement.BMI, 1);          // BMI: 68 / (1.7^2) = 23.5
			Assert.Equal(0.0f, currentMeasurement.WeightChangeKg);   // Pierwszy pomiar = 0 zmiany
		}

		/// <summary>
		/// Weryfikuje obsługę niekompletnego profilu użytkownika (brak wzrostu).
		/// Test sprawdzający graceful degradation funkcjonalności przy niepełnych danych.
		/// </summary>
		/// <remarks>
		/// <para><strong>Scenariusz testowy:</strong></para>
		/// <list type="bullet">
		/// <item>Profil użytkownika nie zawiera wzrostu (HeightCm = null)</item>
		/// <item>Dostępne są pomiary wagi do porównania</item>
		/// <item>Cel wagowy może być ustawiony mimo braku wzrostu</item>
		/// </list>
		/// <para><strong>Oczekiwane zachowanie:</strong></para>
		/// <list type="bullet">
		/// <item>BMI = 0 (nie może być obliczone bez wzrostu)</item>
		/// <item>Zmiana wagi obliczona normalnie (nie wymaga wzrostu)</item>
		/// </list>
		/// <para><strong>UX Consideration:</strong> Aplikacja pozostaje funkcjonalna nawet przy niepełnych danych,
		/// a użytkownik może stopniowo uzupełniać profil.</para>
		/// </remarks>
		[Fact]
		public void FillCalculatedFields_WithNoHeight_ShouldNotCalculateBMI()
		{
			// Arrange - profil bez wzrostu
			var userProfile = CreateUserProfile(heightCm: null, targetWeight: 70);
			var previousMeasurement = CreateMeasurement(weightKg: 73.0f);
			var currentMeasurement = CreateMeasurement(weightKg: 72.0f);

			// Act - wypełnienie pól z niepełnymi danymi
			_service.FillCalculatedFields(currentMeasurement, userProfile, previousMeasurement);

			// Assert - weryfikacja częściowej funkcjonalności
			Assert.Equal(0f, currentMeasurement.BMI);               // BMI nie obliczone (brak wzrostu)
			Assert.Equal(-1.0f, currentMeasurement.WeightChangeKg, 1); // Zmiana nadal obliczona
		}

		/// <summary>
		/// Weryfikuje obsługę przypadków granicznych w kalkulacji BMI.
		/// Test parametryzowany sprawdzający skrajne, ale biologicznie możliwe wartości.
		/// </summary>
		/// <param name="weight">Masa ciała w skrajnym zakresie.</param>
		/// <param name="height">Wzrost w skrajnym zakresie.</param>
		/// <param name="expectedBMI">Oczekiwana wartość BMI.</param>
		/// <remarks>
		/// <para><strong>Testowane przypadki graniczne:</strong></para>
		/// <list type="table">
		/// <item>
		/// <term>Normalny przypadek</term>
		/// <description>72.8kg, 175cm → BMI 23.8 (przypadek referencyjny)</description>
		/// </item>
		/// <item>
		/// <term>Niska waga/wzrost</term>
		/// <description>50kg, 150cm → BMI 22.2 (małe osoby, dzieci)</description>
		/// </item>
		/// <item>
		/// <term>Wysoka waga/wzrost</term>
		/// <description>120kg, 200cm → BMI 30.0 (wysokie osoby, sportowcy)</description>
		/// </item>
		/// </list>
		/// <para><strong>Cel testów:</strong> Zapewnienie stabilności algorytmu dla całego spektrum
		/// ludzkich parametrów antropometrycznych.</para>
		/// </remarks>
		[Theory]
		[InlineData(72.8, 175, 23.8f)]  // Normalny case
		[InlineData(50.0, 150, 22.2f)]  // Niska waga/wzrost
		[InlineData(120, 200, 30.0f)]   // Wysoka waga/wzrost
		public void CalculateBMI_EdgeCases_ShouldHandleCorrectly(float weight, float height, float expectedBMI)
		{
			// Act - kalkulacja dla przypadków granicznych
			var result = _service.CalculateBMI(weight, height);

			// Assert - weryfikacja stabilności algorytmu
			Assert.Equal(expectedBMI, result, 1);
		}

		/// <summary>
		/// Weryfikuje obsługę ekstremalnych zmian masy ciała.
		/// Test sprawdzający stabilność algorytmu przy bardzo dużych różnicach wagowych.
		/// </summary>
		/// <remarks>
		/// <para><strong>Scenariusz testowy:</strong></para>
		/// <list type="bullet">
		/// <item>Obecna waga: 150.7kg</item>
		/// <item>Poprzednia waga: 45.3kg</item>
		/// <item>Różnica: +105.4kg (ekstremalnie duża zmiana)</item>
		/// </list>
		/// <para><strong>Kontekst biznesowy:</strong></para>
		/// <list type="bullet">
		/// <item>Błędy w danych wejściowych</item>
		/// <item>Zmiana jednostek miary przez użytkownika</item>
		/// <item>Długoterminowe zmiany (lata bez pomiarów)</item>
		/// <item>Przypadki medyczne (ciąża, choroby)</item>
		/// </list>
		/// <para><strong>Oczekiwane zachowanie:</strong> Algorytm nie zawiesza się i zwraca matematycznie
		/// poprawny wynik, pozostawiając walidację biznesową wyższym warstwom.</para>
		/// </remarks>
		[Fact]
		public void CalculateWeightChange_WithExtremeValues_ShouldHandleCorrectly()
		{
			// Arrange - ekstremalne wartości wagowe
			float currentWeight = 150.7f;
			float previousWeight = 45.3f;

			// Act - kalkulacja ekstremalnej zmiany
			var result = _service.CalculateWeightChange(currentWeight, previousWeight);

			// Assert - weryfikacja stabilności algorytmu
			Assert.Equal(105.4f, result, 1); // Ekstremalny ale możliwy przypadek
		}

		#region Helper Methods

		/// <summary>
		/// Factory method tworząca profil użytkownika do celów testowych.
		/// Generuje kompletny obiekt <see cref="UserProfile"/> z domyślnymi wartościami dla testów.
		/// </summary>
		/// <param name="heightCm">Wzrost użytkownika w centymetrach (opcjonalny).</param>
		/// <param name="targetWeight">Docelowa waga użytkownika w kilogramach (opcjonalna).</param>
		/// <returns>Instancja <see cref="UserProfile"/> z ustawionymi parametrami testowymi.</returns>
		/// <remarks>
		/// <para><strong>Domyślne wartości testowe:</strong></para>
		/// <list type="bullet">
		/// <item>Płeć: Mężczyzna (do obliczeń BMR)</item>
		/// <item>Wiek: 30 lat (średnia wartość)</item>
		/// <item>Aktywność: Umiarkowanie aktywny</item>
		/// <item>Cel: Utrata wagi</item>
		/// </list>
		/// </remarks>
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

		/// <summary>
		/// Factory method tworząca pomiar wagi do celów testowych.
		/// Generuje obiekt <see cref="WeightMeasurement"/> z ustawioną wagą i domyślnymi metadanymi.
		/// </summary>
		/// <param name="weightKg">Masa ciała w kilogramach.</param>
		/// <returns>Instancja <see cref="WeightMeasurement"/> gotowa do użycia w testach.</returns>
		/// <remarks>
		/// <para><strong>Automatycznie ustawiane wartości:</strong></para>
		/// <list type="bullet">
		/// <item>ID: Nowy GUID</item>
		/// <item>UserId: "test-user" (stały identyfikator testowy)</item>
		/// <item>Data pomiaru: Dzisiejsza data</item>
		/// <item>Timestamps: Aktualny czas UTC</item>
		/// </list>
		/// </remarks>
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

		#endregion
	}
}