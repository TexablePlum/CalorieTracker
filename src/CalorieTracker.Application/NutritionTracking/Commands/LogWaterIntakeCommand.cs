// Plik LogWaterIntakeCommand.cs - komenda dodawania wpisu spożycia wody.

using CalorieTracker.Domain.Enums;

namespace CalorieTracker.Application.NutritionTracking.Commands
{
	/// <summary>
	/// Komenda reprezentująca żądanie dodania nowego wpisu spożycia wody do dziennika użytkownika.
	/// Zawiera wszystkie niezbędne dane do utworzenia wpisu WaterIntakeLogEntry.
	/// </summary>
	public class LogWaterIntakeCommand
	{
		/// <summary>
		/// Identyfikator użytkownika dodającego wpis spożycia wody.
		/// Ustawiany automatycznie na podstawie zalogowanego użytkownika.
		/// </summary>
		public string UserId { get; set; } = null!;

		/// <summary>
		/// Ilość wypitej wody w mililitrach.
		/// </summary>
		public float AmountMilliliters { get; set; }

		/// <summary>
		/// Data i czas spożycia wody (opcjonalny - domyślnie teraz).
		/// </summary>
		public DateTime? ConsumedAt { get; set; }

		/// <summary>
		/// Opcjonalne notatki użytkownika dotyczące spożycia wody.
		/// </summary>
		public string? Notes { get; set; }
	}
}