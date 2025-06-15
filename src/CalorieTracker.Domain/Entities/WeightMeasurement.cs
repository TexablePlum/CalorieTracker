// Plik WeightMeasurement.cs - definicja encji pomiaru masy ciała użytkownika.
// Reprezentuje zapis pojedynczego pomiaru wagi użytkownika wraz z powiązanymi metrykami.

using System.ComponentModel.DataAnnotations;

namespace CalorieTracker.Domain.Entities
{
	/// &lt;summary>
	/// Encja reprezentująca pomiar masy ciała użytkownika.
	/// Zawiera informacje o wadze w danym dniu, obliczone BMI oraz zmianę wagi od poprzedniego pomiaru.
	/// &lt;/summary>
	public class WeightMeasurement
	{
		/// &lt;summary>
		/// Unikalny identyfikator GUID pomiaru masy ciała.
		/// Jest kluczem głównym encji.
		/// &lt;/summary>
		[Key]
		public Guid Id { get; set; } = Guid.NewGuid();

		/// <summary>
		/// Identyfikator użytkownika, którego dotyczy ten pomiar.
		/// Pole wymagane.
		/// </summary>
		[Required]
		public string UserId { get; set; } = null!;

		/// <summary>
		/// Data wykonania pomiaru masy ciała.
		/// Pole wymagane.
		/// </summary>
		[Required]
		public DateOnly MeasurementDate { get; set; }

		/// <summary>
		/// Masa ciała w kilogramach.
		/// Pole wymagane, wartość w zakresie od 20 do 500.
		/// </summary>
		[Required]
		[Range(20, 500)]
		public float WeightKg { get; set; }

		/// <summary>
		/// Wartość BMI (Body Mass Index) obliczona na podstawie <see cref="WeightKg"/>
		/// i wzrostu z <see cref="UserProfile"/>.
		/// Wartość w zakresie od 10 do 80.
		/// </summary>
		[Range(10, 80)]
		public float BMI { get; set; }

		/// <summary>
		/// Zmiana masy ciała w kilogramach od poprzedniego zarejestrowanego pomiaru.
		/// Może być wartością ujemną, wskazuje na spadek wagi.
		/// Wartość w zakresie od -50 do 50.
		/// </summary>
		[Range(-50, 50)]
		public float WeightChangeKg { get; set; }

		/// <summary>
		/// Data i czas utworzenia rekordu pomiaru wagi.
		/// Domyślnie ustawiane na aktualny czas UTC.
		/// </summary>
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		/// <summary>
		/// Data i czas ostatniej aktualizacji rekordu pomiaru wagi.
		/// Domyślnie ustawiane na aktualny czas UTC.
		/// </summary>
		public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

		// === RELACJE ===

		/// <summary>
		/// Właściwość nawigacyjna do użytkownika <see cref="ApplicationUser"/>, którego dotyczy ten pomiar.
		/// </summary>
		public ApplicationUser User { get; set; } = null!;
	}
}