using CalorieTracker.Application.Interfaces;
using CalorieTracker.Application.NutritionTracking.Commands;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.NutritionTracking.Handlers
{
	/// <summary>
	/// Handler odpowiedzialny za przetwarzanie komendy usuwania wpisu spożycia wody.
	/// </summary>
	public class DeleteWaterIntakeHandler
	{
		/// <summary>
		/// Kontekst bazy danych aplikacji.
		/// </summary>
		private readonly IAppDbContext _db;

		/// <summary>
		/// Inicjalizuje nową instancję handlera usuwania wpisu spożycia wody.
		/// </summary>
		public DeleteWaterIntakeHandler(IAppDbContext db)
		{
			_db = db;
		}

		/// <summary>
		/// Przetwarza komendę usuwania wpisu spożycia wody.
		/// </summary>
		/// <param name="command">Komenda z ID wpisu do usunięcia i ID użytkownika.</param>
		/// <returns>True jeśli usunięcie powiodło się, false jeśli wpis nie istnieje lub brak uprawnień.</returns>
		public async Task<bool> Handle(DeleteWaterIntakeCommand command)
		{
			// Znajduje wpis spożycia wody
			var waterIntakeEntry = await _db.WaterIntakeLogEntries
				.FirstOrDefaultAsync(w => w.Id == command.WaterIntakeLogEntryId);

			// Sprawdza istnienie i uprawnienia
			if (waterIntakeEntry == null || waterIntakeEntry.UserId != command.UserId)
				return false;

			// Usuwa wpis
			_db.WaterIntakeLogEntries.Remove(waterIntakeEntry);
			await _db.SaveChangesAsync();

			return true;
		}
	}
}