// Plik WeightMeasurementRecalculationService.cs - serwis aplikacji do przeliczania pomiarów masy ciała.
// Odpowiada za centralizację logiki przeliczania kalkulowanych pól w sekwencji pomiarów użytkownika.
// Eliminuje duplikację kodu między handlerami Create, Update i Delete dla pomiarów wagi.

using CalorieTracker.Application.Interfaces;
using CalorieTracker.Domain.Entities;
using CalorieTracker.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.WeightMeasurements.Services
{
	/// <summary>
	/// Serwis aplikacji odpowiedzialny za przeliczanie pomiarów masy ciała po operacjach modyfikujących sekwencję pomiarów.
	/// Centralizuje logikę aktualizacji kalkulowanych pól (BMI, zmiany wagi) dla zapewnienia spójności danych
	/// w całej chronologicznej sekwencji pomiarów użytkownika.
	/// </summary>
	/// <remarks>
	/// Serwis jest używany przez handlery Create, Update i Delete dla pomiarów wagi w celu:
	/// <list type="bullet">
	/// <item><description>Przeliczania BMI na podstawie aktualnego wzrostu użytkownika</description></item>
	/// <item><description>Kalkulacji zmian wagi względem poprzedniego pomiaru w sekwencji</description></item>
	/// <item><description>Aktualizacji znaczników czasu modyfikacji (UpdatedAt)</description></item>
	/// <item><description>Zapewnienia spójności kalkulowanych pól po zmianach w sekwencji</description></item>
	/// </list>
	/// 
	/// Każda z metod publicznych jest zoptymalizowana pod konkretny scenariusz:
	/// <list type="bullet">
	/// <item><description><see cref="RecalculateFromDate"/> - dla operacji CREATE (nowy pomiar może wpłynąć na przyszłe)</description></item>
	/// <item><description><see cref="RecalculateFromEarliest"/> - dla operacji UPDATE (zmiana daty może wpłynąć na zakres)</description></item>
	/// <item><description><see cref="RecalculateAfterDate"/> - dla operacji DELETE (usunięcie wpływa tylko na przyszłe)</description></item>
	/// </list>
	/// </remarks>
	public class WeightMeasurementRecalculationService
	{
		/// <summary>
		/// Kontekst bazy danych aplikacji umożliwiający dostęp do pomiarów wagi oraz zapisywanie zmian.
		/// </summary>
		private readonly IAppDbContext _db;

		/// <summary>
		/// Serwis domenowy do kalkulacji BMI i zmian wagi zawierający czystą logikę biznesową.
		/// </summary>
		private readonly WeightAnalysisService _weightAnalysis;

		/// <summary>
		/// Inicjalizuje nową instancję serwisu przeliczania pomiarów masy ciała.
		/// </summary>
		/// <param name="db">Kontekst bazy danych <see cref="IAppDbContext"/> do operacji na pomiarach wagi.</param>
		/// <param name="weightAnalysis">Serwis domenowy <see cref="WeightAnalysisService"/> do kalkulacji BMI i zmian wagi.</param>
		public WeightMeasurementRecalculationService(IAppDbContext db, WeightAnalysisService weightAnalysis)
		{
			_db = db;
			_weightAnalysis = weightAnalysis;
		}

		/// <summary>
		/// Przelicza pomiary masy ciała użytkownika od podanej daty do końca sekwencji.
		/// Używane głównie po operacji CREATE - nowy pomiar może wpłynąć na kalkulacje kolejnych pomiarów.
		/// </summary>
		/// <param name="userId">Unikalny identyfikator użytkownika, którego pomiary mają zostać przeliczone.</param>
		/// <param name="fromDate">Data początkowa - pomiary z datami późniejszymi niż ta data zostaną przeliczone.</param>
		/// <param name="userProfile">Profil użytkownika <see cref="UserProfile"/> zawierający dane niezbędne do kalkulacji BMI (wzrost, płeć, wiek).</param>
		/// <returns>Task reprezentujący operację asynchroniczną przeliczania pomiarów.</returns>
		/// <remarks>
		/// Metoda wykonuje następujące operacje:
		/// <list type="number">
		/// <item><description>Pobiera wszystkie pomiary użytkownika w kolejności chronologicznej</description></item>
		/// <item><description>Filtruje pomiary z datami późniejszymi niż <paramref name="fromDate"/></description></item>
		/// <item><description>Dla każdego pomiaru znajdując poprzedni pomiar w sekwencji chronologicznej</description></item>
		/// <item><description>Przelicza BMI i zmianę wagi używając <see cref="WeightAnalysisService"/></description></item>
		/// <item><description>Aktualizuje znacznik czasu modyfikacji</description></item>
		/// <item><description>Zapisuje wszystkie zmiany w bazie danych</description></item>
		/// </list>
		/// 
		/// Przykład użycia: Po dodaniu pomiaru z dnia 15.06, wszystkie pomiary z dat 16.06, 17.06, itd. zostaną przeliczone.
		/// </remarks>
		/// <exception cref="ArgumentNullException">Rzucany gdy <paramref name="userId"/> lub <paramref name="userProfile"/> jest null.</exception>
		public async Task RecalculateFromDate(string userId, DateOnly fromDate, UserProfile userProfile)
		{
			var allMeasurements = await GetAllUserMeasurementsOrdered(userId);
			var measurementsToRecalculate = allMeasurements.Where(m => m.MeasurementDate > fromDate).ToList();
			await RecalculateMeasurements(measurementsToRecalculate, allMeasurements, userProfile);
		}

		/// <summary>
		/// Przelicza pomiary masy ciała użytkownika od najwcześniejszej z dwóch podanych dat.
		/// Używane głównie po operacji UPDATE - zmiana daty pomiaru może wpłynąć na pomiary w szerokim zakresie.
		/// </summary>
		/// <param name="userId">Unikalny identyfikator użytkownika, którego pomiary mają zostać przeliczone.</param>
		/// <param name="date1">Pierwsza data do porównania (np. stara data pomiaru przed aktualizacją).</param>
		/// <param name="date2">Druga data do porównania (np. nowa data pomiaru po aktualizacji).</param>
		/// <param name="userProfile">Profil użytkownika <see cref="UserProfile"/> zawierający dane niezbędne do kalkulacji BMI.</param>
		/// <returns>Task reprezentujący operację asynchroniczną przeliczania pomiarów.</returns>
		/// <remarks>
		/// Metoda automatycznie określa, która z dat jest wcześniejsza i rozpoczyna przeliczanie od tej daty (włącznie).
		/// Jest to konieczne po zmianie daty pomiaru, ponieważ:
		/// <list type="bullet">
		/// <item><description>Zmiana daty z 20.06 na 15.06 wpływa na pomiary od 15.06 wzwyż</description></item>
		/// <item><description>Zmiana daty z 15.06 na 20.06 wpływa na pomiary od 15.06 wzwyż</description></item>
		/// <item><description>W obu przypadkach bezpieczne jest przeliczenie od najwcześniejszej daty</description></item>
		/// </list>
		/// 
		/// Przykład: UPDATE pomiaru z 20.06 na 15.06 → przelicza pomiary od 15.06 włącznie.
		/// </remarks>
		/// <exception cref="ArgumentNullException">Rzucany gdy <paramref name="userId"/> lub <paramref name="userProfile"/> jest null.</exception>
		public async Task RecalculateFromEarliest(string userId, DateOnly date1, DateOnly date2, UserProfile userProfile)
		{
			var earliestDate = date1 < date2 ? date1 : date2;
			var allMeasurements = await GetAllUserMeasurementsOrdered(userId);
			var measurementsToRecalculate = allMeasurements.Where(m => m.MeasurementDate >= earliestDate).ToList();
			await RecalculateMeasurements(measurementsToRecalculate, allMeasurements, userProfile);
		}

		/// <summary>
		/// Przelicza pomiary masy ciała użytkownika następujące po podanej dacie.
		/// Używane głównie po operacji DELETE - usunięcie pomiaru wpływa tylko na kalkulacje późniejszych pomiarów.
		/// </summary>
		/// <param name="userId">Unikalny identyfikator użytkownika, którego pomiary mają zostać przeliczone.</param>
		/// <param name="afterDate">Data graniczna - przeliczane będą tylko pomiary z datami późniejszymi (bez włączenia tej daty).</param>
		/// <param name="userProfile">Profil użytkownika <see cref="UserProfile"/> zawierający dane niezbędne do kalkulacji BMI.</param>
		/// <returns>Task reprezentujący operację asynchroniczną przeliczania pomiarów.</returns>
		/// <remarks>
		/// Po usunięciu pomiaru konieczne jest przeliczenie pomiarów następujących po nim, ponieważ:
		/// <list type="bullet">
		/// <item><description>Zmieniają się poprzednie pomiary w sekwencji dla kalkulacji zmian wagi</description></item>
		/// <item><description>Pomiary wcześniejsze od usuniętego nie wymagają aktualizacji</description></item>
		/// <item><description>Sam usunięty pomiar już nie istnieje, więc nie jest przeliczany</description></item>
		/// </list>
		/// 
		/// Przykład: DELETE pomiaru z dnia 15.06 → przelicza pomiary z dat 16.06, 17.06, 20.06, itd.
		/// ale NIE przelicza pomiarów z 14.06, 10.06 ani samego 15.06.
		/// </remarks>
		/// <exception cref="ArgumentNullException">Rzucany gdy <paramref name="userId"/> lub <paramref name="userProfile"/> jest null.</exception>
		public async Task RecalculateAfterDate(string userId, DateOnly afterDate, UserProfile userProfile)
		{
			var allMeasurements = await GetAllUserMeasurementsOrdered(userId);
			var measurementsToRecalculate = allMeasurements.Where(m => m.MeasurementDate > afterDate).ToList();
			await RecalculateMeasurements(measurementsToRecalculate, allMeasurements, userProfile);
		}

		/// <summary>
		/// Pobiera wszystkie pomiary masy ciała użytkownika w kolejności chronologicznej.
		/// Metoda pomocnicza zapewniająca spójne sortowanie pomiarów dla wszystkich operacji przeliczania.
		/// </summary>
		/// <param name="userId">Unikalny identyfikator użytkownika, którego pomiary mają zostać pobrane.</param>
		/// <returns>
		/// Task zwracający listę <see cref="WeightMeasurement"/> uporządkowaną chronologicznie:
		/// <list type="bullet">
		/// <item><description>Pierwsze kryterium: data pomiaru (rosnąco)</description></item>
		/// <item><description>Drugie kryterium: data utworzenia rekordu (rosnąco) - dla rozróżnienia pomiarów z tej samej daty</description></item>
		/// </list>
		/// </returns>
		/// <remarks>
		/// Sortowanie po <see cref="WeightMeasurement.CreatedAt"/> jako drugie kryterium jest kluczowe
		/// w przypadku gdy użytkownik wprowadzi kilka pomiarów z tą samą datą - zapewnia to deterministyczną kolejność.
		/// </remarks>
		private async Task<List<WeightMeasurement>> GetAllUserMeasurementsOrdered(string userId)
		{
			return await _db.WeightMeasurements
				.Where(w => w.UserId == userId)
				.OrderBy(w => w.MeasurementDate)
				.ThenBy(w => w.CreatedAt)
				.ToListAsync();
		}

		/// <summary>
		/// Wykonuje rzeczywiste przeliczenie kalkulowanych pól dla podanej listy pomiarów.
		/// Główna metoda zawierająca logikę przeliczania używana przez wszystkie publiczne metody serwisu.
		/// </summary>
		/// <param name="measurementsToRecalculate">Lista pomiarów <see cref="WeightMeasurement"/> do przeliczenia.</param>
		/// <param name="allMeasurements">Kompletna lista wszystkich pomiarów użytkownika (dla kontekstu do znajdowania poprzednich pomiarów).</param>
		/// <param name="userProfile">Profil użytkownika <see cref="UserProfile"/> z danymi do kalkulacji BMI.</param>
		/// <returns>Task reprezentujący operację asynchroniczną przeliczania i zapisywania zmian.</returns>
		/// <remarks>
		/// Dla każdego pomiaru do przeliczenia metoda:
		/// <list type="number">
		/// <item><description>Znajduje poprzedni pomiar w sekwencji chronologicznej (lub null dla pierwszego pomiaru)</description></item>
		/// <item><description>Wywołuje <see cref="WeightAnalysisService.FillCalculatedFields"/> do obliczenia BMI i zmiany wagi</description></item>
		/// <item><description>Aktualizuje znacznik czasu modyfikacji na aktualny czas UTC</description></item>
		/// </list>
		/// 
		/// Po przetworzeniu wszystkich pomiarów zapisuje zmiany w bazie danych jedną transakcją.
		/// 
		/// Logika znajdowania poprzedniego pomiaru:
		/// - Bierze pomiary z datami wcześniejszymi niż aktualny pomiar
		/// - Sortuje malejąco po dacie pomiaru, następnie po dacie utworzenia
		/// - Zwraca pierwszy (najnowszy) pomiar lub null jeśli nie ma wcześniejszych
		/// </remarks>
		private async Task RecalculateMeasurements(List<WeightMeasurement> measurementsToRecalculate,
			List<WeightMeasurement> allMeasurements, UserProfile userProfile)
		{
			foreach (var measurement in measurementsToRecalculate)
			{
				var previousMeasurement = allMeasurements
					.Where(m => m.MeasurementDate < measurement.MeasurementDate)
					.OrderByDescending(m => m.MeasurementDate)
					.ThenByDescending(m => m.CreatedAt)
					.FirstOrDefault();

				_weightAnalysis.FillCalculatedFields(measurement, userProfile, previousMeasurement);
				measurement.UpdatedAt = DateTime.UtcNow;
			}

			await _db.SaveChangesAsync();
		}
	}
}