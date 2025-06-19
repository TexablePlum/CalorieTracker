// Plik DeleteMealLogHandler.cs - handler komendy usuwania wpisu posiłku.

using CalorieTracker.Application.Interfaces;
using CalorieTracker.Application.NutritionTracking.Commands;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.NutritionTracking.Handlers
{
	/// <summary>
	/// Handler odpowiedzialny za przetwarzanie komendy usuwania wpisu posiłku.
	/// Zawiera walidację uprawnień użytkownika przed usunięciem.
	/// </summary>
	public class DeleteMealLogHandler
	{
		/// <summary>
		/// Kontekst bazy danych aplikacji.
		/// </summary>
		private readonly IAppDbContext _db;

		/// <summary>
		/// Inicjalizuje nową instancję handlera usuwania wpisu posiłku.
		/// </summary>
		public DeleteMealLogHandler(IAppDbContext db)
		{
			_db = db;
		}

		/// <summary>
		/// Przetwarza komendę usuwania wpisu posiłku z dziennika.
		/// </summary>
		/// <param name="command">Komenda z ID wpisu do usunięcia i ID użytkownika.</param>
		/// <returns>True jeśli usunięcie powiodło się, false jeśli wpis nie istnieje lub brak uprawnień.</returns>
		public async Task<bool> Handle(DeleteMealLogCommand command)
		{
			// Znajduje wpis posiłku
			var mealLogEntry = await _db.MealLogEntries
				.FirstOrDefaultAsync(m => m.Id == command.MealLogEntryId);

			// Sprawdza istnienie i uprawnienia
			if (mealLogEntry == null || mealLogEntry.UserId != command.UserId)
				return false;

			// Usuwa wpis
			_db.MealLogEntries.Remove(mealLogEntry);
			await _db.SaveChangesAsync();

			return true;
		}
	}
}