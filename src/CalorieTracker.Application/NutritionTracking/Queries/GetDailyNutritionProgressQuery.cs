// Plik GetDailyNutritionProgressQuery.cs - zapytanie o dzienny postęp żywieniowy.

namespace CalorieTracker.Application.NutritionTracking.Queries
{
	/// <summary>
	/// Zapytanie o dzienny postęp żywieniowy użytkownika.
	/// Zwraca cele, spożycie, pozostałe potrzeby i listę posiłków z danego dnia.
	/// </summary>
	public class GetDailyNutritionProgressQuery
	{
		/// <summary>
		/// Identyfikator użytkownika.
		/// </summary>
		public string UserId { get; set; } = null!;

		/// <summary>
		/// Data dla której pobierany jest postęp (opcjonalnie - domyślnie dzisiaj).
		/// </summary>
		public DateOnly? Date { get; set; }

		/// <summary>
		/// Inicjalizuje nowe zapytanie o postęp żywieniowy.
		/// </summary>
		/// <param name="userId">ID użytkownika.</param>
		/// <param name="date">Data (opcjonalnie).</param>
		public GetDailyNutritionProgressQuery(string userId, DateOnly? date = null)
		{
			UserId = userId;
			Date = date;
		}

		/// <summary>
		/// Konstruktor bezparametrowy dla model bindingu.
		/// </summary>
		public GetDailyNutritionProgressQuery() { }

		/// <summary>
		/// Zwraca efektywną datę zapytania (dzisiaj jeśli nie podano).
		/// </summary>
		public DateOnly EffectiveDate => Date ?? DateOnly.FromDateTime(DateTime.Today);
	}
}