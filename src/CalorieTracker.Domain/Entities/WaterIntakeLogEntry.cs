// Plik WaterIntakeLogEntry.cs - definicja encji wpisu spożycia wody.
// Reprezentuje pojedynczy wpis spożycia wody w dzienniku użytkownika.

using System.ComponentModel.DataAnnotations;

namespace CalorieTracker.Domain.Entities
{
	/// <summary>
	/// Encja reprezentująca pojedynczy wpis spożycia wody w dzienniku użytkownika.
	/// Zawiera informacje o ilości wypitej wody i czasie spożycia.
	/// Służy do kalkulacji dziennego spożycia płynów i monitorowania hidratacji.
	/// </summary>
	public class WaterIntakeLogEntry
	{
		/// <summary>
		/// Unikalny identyfikator GUID wpisu spożycia wody.
		/// Jest kluczem głównym encji.
		/// </summary>
		[Key]
		public Guid Id { get; set; } = Guid.NewGuid();

		/// <summary>
		/// Identyfikator użytkownika, którego dotyczy ten wpis spożycia wody.
		/// Pole wymagane.
		/// </summary>
		[Required]
		public string UserId { get; set; } = null!;

		/// <summary>
		/// Ilość wypitej wody w mililitrach.
		/// Typowe wartości: 250ml (szklanka), 500ml (butelka), 1000ml (duża butelka).
		/// </summary>
		[Required]
		[Range(1, 5000, ErrorMessage = "Ilość wody musi być między 1ml a 5000ml")]
		public float AmountMilliliters { get; set; }

		/// <summary>
		/// Data i czas rzeczywistego spożycia wody.
		/// Domyślnie ustawiane na czas utworzenia wpisu.
		/// </summary>
		[Required]
		public DateTime ConsumedAt { get; set; } = DateTime.UtcNow;

		/// <summary>
		/// Opcjonalne notatki użytkownika dotyczące spożycia wody.
		/// Może zawierać informacje o rodzaju napoju (woda, herbata, kawa, etc.).
		/// </summary>
		[MaxLength(200)]
		public string? Notes { get; set; }

		/// <summary>
		/// Data i czas utworzenia wpisu w systemie.
		/// </summary>
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		// Właściwości nawigacyjne

		/// <summary>
		/// Właściwość nawigacyjna do powiązanego użytkownika.
		/// </summary>
		public ApplicationUser User { get; set; } = null!;

		/// <summary>
		/// Konwertuje ilość wody z mililitrów na litry.
		/// </summary>
		public float AmountLiters => AmountMilliliters / 1000f;

		/// <summary>
		/// Sprawdza czy wpis został utworzony dzisiaj.
		/// </summary>
		public bool IsToday => DateOnly.FromDateTime(ConsumedAt) == DateOnly.FromDateTime(DateTime.Now);
	}
}