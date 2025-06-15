// Plik GeneratePasswordResetHandler.cs - implementacja handlera resetowania hasła.
// Odpowiada za generowanie i wysyłkę kodu weryfikacyjnego wymaganego do procesu resetowania hasła użytkownika.

using CalorieTracker.Application.Auth.Interfaces;
using CalorieTracker.Application.Interfaces;
using CalorieTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace CalorieTracker.Application.Auth.Handlers
{
	/// <summary>
	/// Handler odpowiedzialny za proces generowania kodu resetowania hasła.
	/// Zarządza cyklem życia kodów resetujących i koordynuje wysyłkę emaili weryfikacyjnych.
	/// </summary>
	public class GeneratePasswordResetHandler
	{
		/// <summary>
		/// Kontekst bazy danych do zarządzania kodami resetującymi hasło.
		/// </summary>
		private readonly IAppDbContext _db;

		/// <summary>
		/// Serwis do wysyłania wiadomości email.
		/// </summary>
		private readonly IEmailSender _emailSender;

		/// <summary>
		/// Inicjalizuje nową instancję handlera resetowania hasła.
		/// </summary>
		/// <param name="db">Kontekst bazy danych <see cref="IAppDbContext"/>.</param>
		/// <param name="emailSender">Serwis wysyłki emaili <see cref="IEmailSender"/>.</param>
		public GeneratePasswordResetHandler(IAppDbContext db, IEmailSender emailSender)
		{
			_db = db;
			_emailSender = emailSender;
		}

		/// <summary>
		/// Główna metoda handlera wykonująca proces generowania i wysyłki kodu resetującego hasło.
		/// </summary>
		/// <param name="user">Użytkownik <see cref="ApplicationUser"/> żądający resetu hasła.</param>
		/// <returns>Task reprezentujący operację asynchroniczną.</returns>
		public async Task Handle(ApplicationUser user)
		{
			// Generuje 6-cyfrowy kod weryfikacyjny
			var code = RandomNumberGenerator.GetInt32(100000, 999999).ToString();

			// Usuwa wszystkie istniejące kody resetu hasła dla danego użytkownika
			var old = await _db.PasswordResets
				.Where(r => r.UserId == user.Id)
				.ToListAsync();

			_db.PasswordResets.RemoveRange(old);

			// Tworzy i zapisuje nowy rekord resetu hasła
			var reset = new PasswordReset
			{
				UserId = user.Id,
				Code = code,
				ExpiresAt = DateTime.UtcNow.AddMinutes(10) // Kod ważny przez 10 minut
			};

			_db.PasswordResets.Add(reset);
			await _db.SaveChangesAsync();

			// Wysyła email z kodem resetującym hasło
			await _emailSender.SendAsync(
				user.Email!,
				"Resetowanie hasła – CalorieTracker",
				$"Twój kod resetowania hasła to: <b>{code}</b><br/>Kod jest ważny przez 10 minut."
			);
		}
	}
}