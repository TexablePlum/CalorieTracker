// Plik DeleteMealLogCommand.cs - komenda usuwania wpisu posiłku.

namespace CalorieTracker.Application.NutritionTracking.Commands
{
	/// <summary>
	/// Komenda usuwania wpisu posiłku z dziennika żywieniowego.
	/// Zawiera walidację uprawnień użytkownika.
	/// </summary>
	public class DeleteMealLogCommand
	{
		/// <summary>
		/// Identyfikator wpisu posiłku do usunięcia.
		/// </summary>
		public Guid MealLogEntryId { get; set; }

		/// <summary>
		/// Identyfikator użytkownika (walidacja uprawnień).
		/// </summary>
		public string UserId { get; set; } = null!;

		/// <summary>
		/// Inicjalizuje nową komendę usuwania posiłku.
		/// </summary>
		/// <param name="mealLogEntryId">ID wpisu do usunięcia.</param>
		/// <param name="userId">ID użytkownika.</param>
		public DeleteMealLogCommand(Guid mealLogEntryId, string userId)
		{
			MealLogEntryId = mealLogEntryId;
			UserId = userId;
		}

		/// <summary>
		/// Konstruktor bezparametrowy dla model bindingu.
		/// </summary>
		public DeleteMealLogCommand() { }
	}
}