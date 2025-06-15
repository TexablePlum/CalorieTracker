// Plik EmailConfirmation.cs - definicja encji potwierdzenia adresu e-mail.
// Reprezentuje rekord służący do weryfikacji adresu e-mail użytkownika podczas rejestracji lub zmiany danych.

namespace CalorieTracker.Domain.Entities
{
	/// &lt;summary>
	/// Reprezentacja rekordu potwierdzenia adresu e-mail użytkownika.
	/// Używana do weryfikacji autentyczności adresu e-mail.
	/// &lt;/summary>
	public class EmailConfirmation
	{
		/// &lt;summary>
		/// Unikalny identyfikator rekordu potwierdzenia.
		/// &lt;/summary>
		public int Id { get; set; }

		/// <summary>
		/// Identyfikator użytkownika, dla którego generowane jest potwierdzenie.
		/// </summary>
		public string UserId { get; set; } = null!;

		/// <summary>
		/// Unikalny kod potwierdzający wysyłany na adres e-mail użytkownika.
		/// </summary>
		public string Code { get; set; } = null!;

		/// <summary>
		/// Data i czas wygaśnięcia kodu potwierdzającego.
		/// </summary>
		public DateTime ExpiresAt { get; set; }

		/// <summary>
		/// Właściwość nawigacyjna do powiązanego użytkownika <see cref="ApplicationUser"/>.
		/// </summary>
		public ApplicationUser User { get; set; } = null!;
	}
}