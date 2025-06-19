// Plik UpdateMealLogHandler.cs - handler komendy aktualizacji wpisu posiłku.

using CalorieTracker.Application.Interfaces;
using CalorieTracker.Application.NutritionTracking.Commands;
using CalorieTracker.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.NutritionTracking.Handlers
{
	/// <summary>
	/// Handler odpowiedzialny za przetwarzanie komendy aktualizacji wpisu posiłku.
	/// Zarządza walidacją uprawnień, modyfikacją danych oraz przeliczaniem wartości odżywczych.
	/// </summary>
	public class UpdateMealLogHandler
	{
		/// <summary>
		/// Kontekst bazy danych aplikacji.
		/// </summary>
		private readonly IAppDbContext _db;

		/// <summary>
		/// Serwis domenowy do śledzenia żywienia i kalkulacji wartości odżywczych.
		/// </summary>
		private readonly NutritionTrackingService _nutritionTrackingService;

		/// <summary>
		/// Inicjalizuje nową instancję handlera aktualizacji wpisu posiłku.
		/// </summary>
		public UpdateMealLogHandler(IAppDbContext db, NutritionTrackingService nutritionTrackingService)
		{
			_db = db;
			_nutritionTrackingService = nutritionTrackingService;
		}

		/// <summary>
		/// Przetwarza komendę aktualizacji wpisu posiłku.
		/// </summary>
		/// <param name="command">Komenda z nowymi danymi wpisu.</param>
		/// <returns>True jeśli aktualizacja powiodła się, false jeśli wpis nie istnieje lub brak uprawnień.</returns>
		public async Task<bool> Handle(UpdateMealLogCommand command)
		{
			// Znajduje wpis posiłku z relacjami
			var mealLogEntry = await _db.MealLogEntries
				.Include(m => m.Product)
				.Include(m => m.Recipe)
				.ThenInclude(r => r!.Ingredients)
				.ThenInclude(i => i.Product)
				.FirstOrDefaultAsync(m => m.Id == command.MealLogEntryId);

			// Sprawdza istnienie i uprawnienia
			if (mealLogEntry == null || mealLogEntry.UserId != command.UserId)
				return false;

			// Aktualizuje pola
			mealLogEntry.Quantity = command.Quantity;
			mealLogEntry.MealType = command.MealType;
			mealLogEntry.ConsumedAt = command.ConsumedAt;
			mealLogEntry.Notes = command.Notes;

			// Waliduje zaktualizowane dane
			if (!_nutritionTrackingService.ValidateMealLogEntry(mealLogEntry))
				throw new ArgumentException("Zaktualizowane dane wpisu są nieprawidłowe");

			// Przelicza wartości odżywcze dla nowej ilości
			_nutritionTrackingService.CalculateNutritionForMealEntry(mealLogEntry);

			// Zapisuje zmiany
			await _db.SaveChangesAsync();
			return true;
		}
	}
}