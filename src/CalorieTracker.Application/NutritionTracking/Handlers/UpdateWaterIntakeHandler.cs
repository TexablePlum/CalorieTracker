using CalorieTracker.Application.Interfaces;
using CalorieTracker.Application.NutritionTracking.Commands;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.NutritionTracking.Handlers
{
	/// <summary>
	/// Handler odpowiedzialny za przetwarzanie komendy aktualizacji wpisu spożycia wody.
	/// </summary>
	public class UpdateWaterIntakeHandler
	{
		/// <summary>
		/// Kontekst bazy danych aplikacji.
		/// </summary>
		private readonly IAppDbContext _db;

		/// <summary>
		/// Inicjalizuje nową instancję handlera aktualizacji wpisu spożycia wody.
		/// </summary>
		public UpdateWaterIntakeHandler(IAppDbContext db)
		{
			_db = db;
		}

		/// <summary>
		/// Przetwarza komendę aktualizacji wpisu spożycia wody.
		/// </summary>
		/// <param name="command">Komenda z nowymi danymi wpisu.</param>
		/// <returns>True jeśli aktualizacja powiodła się, false jeśli wpis nie istnieje lub brak uprawnień.</returns>
		public async Task<bool> Handle(UpdateWaterIntakeCommand command)
		{
			// Waliduje komendę
			ValidateCommand(command);

			// Znajduje wpis spożycia wody
			var waterIntakeEntry = await _db.WaterIntakeLogEntries
				.FirstOrDefaultAsync(w => w.Id == command.WaterIntakeLogEntryId);

			// Sprawdza istnienie i uprawnienia
			if (waterIntakeEntry == null || waterIntakeEntry.UserId != command.UserId)
				return false;

			// Aktualizuje dane
			waterIntakeEntry.AmountMilliliters = command.AmountMilliliters;
			waterIntakeEntry.ConsumedAt = command.ConsumedAt;
			waterIntakeEntry.Notes = command.Notes;

			await _db.SaveChangesAsync();
			return true;
		}

		/// <summary>
		/// Waliduje dane komendy aktualizacji.
		/// </summary>
		private static void ValidateCommand(UpdateWaterIntakeCommand command)
		{
			if (string.IsNullOrEmpty(command.UserId))
				throw new ArgumentException("UserId jest wymagane");

			if (command.AmountMilliliters <= 0 || command.AmountMilliliters > 5000)
				throw new ArgumentException("AmountMilliliters musi być w przedziale 1-5000ml");

			if (command.ConsumedAt > DateTime.UtcNow.AddMinutes(5))
				throw new ArgumentException("ConsumedAt nie może być w przyszłości");
		}
	}
}