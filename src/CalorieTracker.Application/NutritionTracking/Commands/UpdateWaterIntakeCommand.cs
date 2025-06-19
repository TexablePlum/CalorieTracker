namespace CalorieTracker.Application.NutritionTracking.Commands
{
	/// <summary>
	/// Komenda reprezentująca żądanie aktualizacji wpisu spożycia wody.
	/// </summary>
	public class UpdateWaterIntakeCommand
	{
		/// <summary>
		/// Identyfikator wpisu spożycia wody do aktualizacji.
		/// </summary>
		public Guid WaterIntakeLogEntryId { get; set; }

		/// <summary>
		/// Identyfikator użytkownika wykonującego aktualizację.
		/// </summary>
		public string UserId { get; set; } = null!;

		/// <summary>
		/// Nowa ilość wypitej wody w mililitrach.
		/// </summary>
		public float AmountMilliliters { get; set; }

		/// <summary>
		/// Nowa data i czas spożycia wody.
		/// </summary>
		public DateTime ConsumedAt { get; set; }

		/// <summary>
		/// Nowe notatki użytkownika.
		/// </summary>
		public string? Notes { get; set; }
	}
}