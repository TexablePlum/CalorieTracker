// Plik LogWaterIntakeHandler.cs - handler dodawania wpisu spożycia wody.

using CalorieTracker.Application.Interfaces;
using CalorieTracker.Application.NutritionTracking.Commands;
using CalorieTracker.Domain.Entities;

namespace CalorieTracker.Application.NutritionTracking.Handlers
{
	/// <summary>
	/// Handler odpowiedzialny za przetwarzanie komendy dodawania nowego wpisu spożycia wody.
	/// Zarządza walidacją i zapisem do bazy danych.
	/// </summary>
	public class LogWaterIntakeHandler
	{
		/// <summary>
		/// Kontekst bazy danych aplikacji.
		/// </summary>
		private readonly IAppDbContext _db;

		/// <summary>
		/// Inicjalizuje nową instancję handlera dodawania wpisu spożycia wody.
		/// </summary>
		public LogWaterIntakeHandler(IAppDbContext db)
		{
			_db = db;
		}

		/// <summary>
		/// Przetwarza komendę dodawania nowego wpisu spożycia wody.
		/// </summary>
		/// <param name="command">Komenda zawierająca dane nowego wpisu spożycia wody.</param>
		/// <returns>Unikalny identyfikator utworzonego wpisu spożycia wody.</returns>
		/// <exception cref="ArgumentException">Gdy dane komendy są nieprawidłowe.</exception>
		public async Task<Guid> Handle(LogWaterIntakeCommand command)
		{
			// Walidacja podstawowych danych komendy
			ValidateCommand(command);

			// Tworzenie nowego wpisu spożycia wody
			var waterIntakeEntry = new WaterIntakeLogEntry
			{
				UserId = command.UserId,
				AmountMilliliters = command.AmountMilliliters,
				ConsumedAt = command.ConsumedAt ?? DateTime.UtcNow,
				Notes = command.Notes
			};

			// Zapis do bazy danych
			_db.WaterIntakeLogEntries.Add(waterIntakeEntry);
			await _db.SaveChangesAsync();

			return waterIntakeEntry.Id;
		}

		/// <summary>
		/// Waliduje podstawowe dane komendy.
		/// </summary>
		private static void ValidateCommand(LogWaterIntakeCommand command)
		{
			if (string.IsNullOrEmpty(command.UserId))
				throw new ArgumentException("UserId jest wymagane");

			if (command.AmountMilliliters <= 0 || command.AmountMilliliters > 5000)
				throw new ArgumentException("AmountMilliliters musi być w przedziale 1-5000ml");

			if (command.ConsumedAt.HasValue && command.ConsumedAt.Value > DateTime.UtcNow.AddMinutes(5))
				throw new ArgumentException("ConsumedAt nie może być w przyszłości");
		}
	}
}
