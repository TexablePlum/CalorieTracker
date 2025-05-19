using CalorieTracker.Application.Auth.Interfaces;
using CalorieTracker.Application.Interfaces;
using CalorieTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace CalorieTracker.Application.Auth.Handlers
{
	public class GenerateEmailConfirmationHandler
	{
		private readonly IAppDbContext _db;
		private readonly IEmailSender _emailSender;

		public GenerateEmailConfirmationHandler(IAppDbContext db, IEmailSender emailSender)
		{
			_db = db;
			_emailSender = emailSender;
		}

		public async Task Handle(ApplicationUser user)
		{
			// generuje kod
			var code = RandomNumberGenerator.GetInt32(100000, 999999).ToString();

			// usuwa stare rekordy (jeśli istnieją)
			var old = await _db.EmailConfirmations
				.Where(c => c.UserId == user.Id)
				.ToListAsync();

			_db.EmailConfirmations.RemoveRange(old);

			// zapisuje nowy rekord
			var confirmation = new EmailConfirmation
			{
				UserId = user.Id,
				Code = code,
				ExpiresAt = DateTime.UtcNow.AddMinutes(10)
			};

			_db.EmailConfirmations.Add(confirmation);
			await _db.SaveChangesAsync();

			// wysyła maila
			await _emailSender.SendAsync(
				user.Email!,
				"Potwierdzenie konta - CalorieTracker",
				$"Twój kod aktywacyjny to: <b>{code}</b><br/>Jest ważny przez 10 minut."
			);
		}
	}
}
