using CalorieTracker.Application.Auth.Interfaces;
using CalorieTracker.Application.Interfaces;
using CalorieTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace CalorieTracker.Application.Auth.Handlers
{
	public class GeneratePasswordResetHandler
	{
		private readonly IAppDbContext _db;
		private readonly IEmailSender _emailSender;

		public GeneratePasswordResetHandler(IAppDbContext db, IEmailSender emailSender)
		{
			_db = db;
			_emailSender = emailSender;
		}

		public async Task Handle(ApplicationUser user)
		{
			var code = RandomNumberGenerator.GetInt32(100000, 999999).ToString();

			// usuwa stare rekordy
			var old = await _db.PasswordResets
				.Where(r => r.UserId == user.Id)
				.ToListAsync();

			_db.PasswordResets.RemoveRange(old);

			var reset = new PasswordReset
			{
				UserId = user.Id,
				Code = code,
				ExpiresAt = DateTime.UtcNow.AddMinutes(10)
			};

			_db.PasswordResets.Add(reset);
			await _db.SaveChangesAsync();

			await _emailSender.SendAsync(
				user.Email!,
				"Resetowanie hasła – CalorieTracker",
				$"Twój kod resetowania hasła to: <b>{code}</b><br/>Kod jest ważny przez 10 minut."
			);
		}
	}
}
