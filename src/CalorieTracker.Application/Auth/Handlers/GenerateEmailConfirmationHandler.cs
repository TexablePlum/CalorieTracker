// Plik GenerateEmailConfirmationHandler.cs - implementacja handlera generowania potwierdzenia email.
// Odpowiada za proces generowania i wysyłki kodu weryfikacyjnego potrzebnego do aktywacji konta użytkownika.

using CalorieTracker.Application.Auth.Interfaces;
using CalorieTracker.Application.Interfaces;
using CalorieTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace CalorieTracker.Application.Auth.Handlers
{
	/// <summary>
	/// Handler odpowiedzialny za generowanie i wysyłkę kodu potwierdzającego email użytkownika.
	/// Zarządza cyklem życia kodów weryfikacyjnych i koordynuje proces wysyłki emaili.
	/// </summary>
	public class GenerateEmailConfirmationHandler
	{
		/// <summary>
		/// Instancja kontekstu bazy danych używana do zarządzania kodami weryfikacyjnymi.
		/// </summary>
		private readonly IAppDbContext _db;

		/// <summary>
		/// Serwis odpowiedzialny za wysyłkę wiadomości email.
		/// </summary>
		private readonly IEmailSender _emailSender;

		/// <summary>
		/// Inicjalizuje nową instancję handlera.
		/// </summary>
		/// <param name="db">Kontekst bazy danych <see cref="IAppDbContext"/>.</param>
		/// <param name="emailSender">Serwis wysyłki emaili <see cref="IEmailSender"/>.</param>
		public GenerateEmailConfirmationHandler(IAppDbContext db, IEmailSender emailSender)
		{
			_db = db;
			_emailSender = emailSender;
		}

		/// <summary>
		/// Główna metoda handlera wykonująca cały proces generowania i wysyłki kodu weryfikacyjnego.
		/// </summary>
		/// <param name="user">Użytkownik <see cref="ApplicationUser"/> wymagający potwierdzenia email.</param>
		/// <returns>Task reprezentujący operację asynchroniczną.</returns>
		public async Task Handle(ApplicationUser user)
		{
			// Generuje 6-cyfrowy kod weryfikacyjny
			var code = RandomNumberGenerator.GetInt32(100000, 999999).ToString();

			// Usuwa wszystkie istniejące kody weryfikacyjne dla danego użytkownika
			var old = await _db.EmailConfirmations
				.Where(c => c.UserId == user.Id)
				.ToListAsync();

			_db.EmailConfirmations.RemoveRange(old);

			// Tworzy i zapisuje nowy rekord potwierdzenia email
			var confirmation = new EmailConfirmation
			{
				UserId = user.Id,
				Code = code,
				ExpiresAt = DateTime.UtcNow.AddMinutes(10) // Kod ważny przez 10 minut
			};

			_db.EmailConfirmations.Add(confirmation);
			await _db.SaveChangesAsync();

			// Wysyła email z kodem weryfikacyjnym do użytkownika
			await _emailSender.SendAsync(
				user.Email!,
				"Potwierdzenie konta - CalorieTracker",
				$"Twój kod aktywacyjny to: <b>{code}</b><br/>Jest ważny przez 10 minut."
			);
		}
	}
}