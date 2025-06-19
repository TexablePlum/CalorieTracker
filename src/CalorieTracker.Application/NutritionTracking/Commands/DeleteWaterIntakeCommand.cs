namespace CalorieTracker.Application.NutritionTracking.Commands
{
	/// <summary>
	/// Komenda reprezentująca żądanie usunięcia wpisu spożycia wody.
	/// </summary>
	public class DeleteWaterIntakeCommand
	{
		/// <summary>
		/// Identyfikator wpisu spożycia wody do usunięcia.
		/// </summary>
		public Guid WaterIntakeLogEntryId { get; }

		/// <summary>
		/// Identyfikator użytkownika wykonującego usunięcie.
		/// </summary>
		public string UserId { get; }

		/// <summary>
		/// Inicjalizuje nową instancję komendy usuwania wpisu spożycia wody.
		/// </summary>
		public DeleteWaterIntakeCommand(Guid waterIntakeLogEntryId, string userId)
		{
			WaterIntakeLogEntryId = waterIntakeLogEntryId;
			UserId = userId;
		}
	}
}