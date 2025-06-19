// Plik GetMealHistoryHandler.cs - handler pobierania posiłków z konkretnego dnia.
// Prosty handler zwracający listę MealLogEntry dla wybranej daty.

using CalorieTracker.Application.Interfaces;
using CalorieTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.NutritionTracking.Handlers
{
	/// <summary>
	/// Handler odpowiedzialny za pobieranie posiłków użytkownika z konkretnego dnia.
	/// Zwraca prostą listę entities zgodnie z wzorcem używanym w aplikacji.
	/// </summary>
	public class GetMealHistoryHandler
	{
		/// <summary>
		/// Kontekst bazy danych aplikacji.
		/// </summary>
		private readonly IAppDbContext _db;

		/// <summary>
		/// Inicjalizuje nową instancję handlera pobierania historii posiłków.
		/// </summary>
		/// <param name="db">Kontekst bazy danych.</param>
		public GetMealHistoryHandler(IAppDbContext db)
		{
			_db = db;
		}

		/// <summary>
		/// Pobiera wszystkie posiłki użytkownika z konkretnego dnia.
		/// Zwraca posiłki posortowane chronologicznie wraz z powiązanymi produktami i przepisami.
		/// </summary>
		/// <param name="userId">Identyfikator użytkownika.</param>
		/// <param name="date">Data do pobrania posiłków.</param>
		/// <returns>Lista posiłków z danego dnia posortowana chronologicznie.</returns>
		/// <exception cref="ArgumentNullException">Gdy userId jest null lub pusty.</exception>
		public async Task<List<MealLogEntry>> GetMealsForDay(string userId, DateOnly date)
		{
			// Walidacja parametrów
			if (string.IsNullOrEmpty(userId))
				throw new ArgumentNullException(nameof(userId), "UserId nie może być pusty");

			// Pobieranie posiłków z konkretnego dnia
			var meals = await _db.MealLogEntries
				.Include(m => m.Product)
				.Include(m => m.Recipe)
				.Where(m => m.UserId == userId &&
						   DateOnly.FromDateTime(m.ConsumedAt) == date)
				.OrderBy(m => m.ConsumedAt)
				.ToListAsync();

			return meals;
		}
	}
}