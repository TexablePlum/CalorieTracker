// Plik PasswordReset.cs - definicja encji resetowania hasła.
// Reprezentuje rekord używany do zarządzania procesem resetowania hasła dla użytkowników.

using System.ComponentModel.DataAnnotations;

namespace CalorieTracker.Domain.Entities
{
	/// &lt;summary>
	/// Reprezentacja rekordu służącego do resetowania hasła użytkownika.
	/// Zawiera unikalny kod i datę wygaśnięcia, powiązany z konkretnym użytkownikiem.
	/// &lt;/summary>
	public class PasswordReset
	{
		/// &lt;summary>
		/// Unikalny identyfikator GUID rekordu resetowania hasła.
		/// Jest kluczem głównym encji.
		/// &lt;/summary>
		[Key]
		public Guid Id { get; set; } = Guid.NewGuid();

		/// <summary>
		/// Identyfikator użytkownika, dla którego generowane jest żądanie resetowania hasła.
		/// </summary>
		public string UserId { get; set; } = null!;

		/// <summary>
		/// Właściwość nawigacyjna do powiązanego użytkownika <see cref="ApplicationUser"/>.
		/// </summary>
		public ApplicationUser User { get; set; } = null!;

		/// <summary>
		/// Unikalny kod resetowania hasła, wysyłany do użytkownika.
		/// </summary>
		public string Code { get; set; } = null!;

		/// <summary>
		/// Data i czas wygaśnięcia kodu resetowania hasła.
		/// </summary>
		public DateTime ExpiresAt { get; set; }
	}
}