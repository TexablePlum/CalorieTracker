// Plik UpdateMealLogCommand.cs - komenda aktualizacji wpisu posiłku.

using CalorieTracker.Domain.Enums;

namespace CalorieTracker.Application.NutritionTracking.Commands
{
	/// <summary>
	/// Komenda aktualizacji istniejącego wpisu posiłku w dzienniku żywieniowym.
	/// Pozwala na modyfikację ilości, typu posiłku, czasu spożycia i notatek.
	/// </summary>
	public class UpdateMealLogCommand
	{
		/// <summary>
		/// Identyfikator wpisu posiłku do aktualizacji.
		/// </summary>
		public Guid MealLogEntryId { get; set; }

		/// <summary>
		/// Identyfikator użytkownika (walidacja uprawnień).
		/// </summary>
		public string UserId { get; set; } = null!;

		/// <summary>
		/// Nowa ilość spożytego produktu/przepisu.
		/// </summary>
		public float Quantity { get; set; }

		/// <summary>
		/// Nowy typ posiłku.
		/// </summary>
		public MealType MealType { get; set; }

		/// <summary>
		/// Nowy czas spożycia posiłku.
		/// </summary>
		public DateTime ConsumedAt { get; set; }

		/// <summary>
		/// Nowe notatki użytkownika.
		/// </summary>
		public string? Notes { get; set; }
	}
}