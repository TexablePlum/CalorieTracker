// Plik LogMealCommand.cs - komenda dodawania posiłku do dziennika żywieniowego.
// Implementuje wzorzec CQRS dla operacji zapisu nowych wpisów posiłków.

using CalorieTracker.Domain.Enums;

namespace CalorieTracker.Application.NutritionTracking.Commands
{
	/// <summary>
	/// Komenda dodawania nowego posiłku do dziennika żywieniowego użytkownika.
	/// Część wzorca CQRS do operacji zapisu danych o spożytych posiłkach.
	/// </summary>
	public class LogMealCommand
	{
		/// <summary>
		/// Identyfikator użytkownika logującego posiłek.
		/// </summary>
		public string UserId { get; set; } = null!;

		/// <summary>
		/// Identyfikator produktu spożywczego (opcjonalny - XOR z RecipeId).
		/// </summary>
		public Guid? ProductId { get; set; }

		/// <summary>
		/// Identyfikator przepisu kulinarnego (opcjonalny - XOR z ProductId).
		/// </summary>
		public Guid? RecipeId { get; set; }

		/// <summary>
		/// Ilość spożytego produktu/przepisu w jednostkach bazowych.
		/// </summary>
		public float Quantity { get; set; }

		/// <summary>
		/// Typ posiłku (śniadanie, obiad, przekąska, etc.).
		/// </summary>
		public MealType MealType { get; set; }

		/// <summary>
		/// Data i czas spożycia posiłku (opcjonalny - domyślnie teraz).
		/// </summary>
		public DateTime? ConsumedAt { get; set; }

		/// <summary>
		/// Opcjonalne notatki użytkownika dotyczące posiłku.
		/// </summary>
		public string? Notes { get; set; }
	}
}