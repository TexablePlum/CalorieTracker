using System.ComponentModel.DataAnnotations;

namespace CalorieTracker.Domain.Entities
{
	/// <summary>
	/// Encja reprezentująca pomiar masy ciała użytkownika
	/// </summary>
	public class WeightMeasurement
	{
		[Key]
		public Guid Id { get; set; } = Guid.NewGuid();

		/// <summary>ID użytkownika którego dotyczy pomiar</summary>
		[Required]
		public string UserId { get; set; } = null!;

		/// <summary>Data pomiaru</summary>
		[Required]
		public DateOnly MeasurementDate { get; set; }

		/// <summary>Masa ciała w kilogramach</summary>
		[Required]
		[Range(20, 500)]
		public float WeightKg { get; set; }

		/// <summary>BMI obliczone na podstawie wzrostu z profilu użytkownika</summary>
		[Range(10, 80)]
		public float BMI { get; set; }

		/// <summary>Zmiana masy ciała od poprzedniego pomiaru (może być ujemna)</summary>
		[Range(-50, 50)]
		public float WeightChangeKg { get; set; }

		/// <summary>Data utworzenia rekordu</summary>
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		/// <summary>Data ostatniej aktualizacji</summary>
		public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

		// === RELACJE ===

		/// <summary>Użytkownik którego dotyczy pomiar</summary>
		public ApplicationUser User { get; set; } = null!;
	}
}