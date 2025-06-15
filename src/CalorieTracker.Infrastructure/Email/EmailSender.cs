// Plik EmailSender.cs - implementacja mechanizmu wysyłania wiadomości e-mail.
// Odpowiada za wysyłanie wiadomości e-mail do użytkowników, wykorzystując bibliotekę MailKit.
using CalorieTracker.Application.Auth.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace CalorieTracker.Infrastructure.Email
{
	/// <summary>
	/// Implementacja interfejsu <see cref="IEmailSender"/> odpowiedzialna za wysyłanie wiadomości e-mail.
	/// Wykorzystuje bibliotekę MailKit do nawiązywania połączenia z serwerem SMTP i przesyłania wiadomości.
	/// </summary>
	public class EmailSender : IEmailSender
	{
		/// <summary>
		/// Prywatne pole tylko do odczytu, przechowujące konfigurację serwera poczty wychodzącej.
		/// </summary>
		private readonly EmailSettings _cfg;

		/// <summary>
		/// Inicjalizuje nową instancję klasy <see cref="EmailSender"/>.
		/// </summary>
		/// <param name="cfg">Obiekt konfiguracji zawierający ustawienia serwera SMTP.</param>
		public EmailSender(IOptions<EmailSettings> cfg) => _cfg = cfg.Value;

		/// <summary>
		/// Wysyła wiadomość e-mail do określonego odbiorcy.
		/// </summary>
		/// <param name="to">Adres e-mail odbiorcy wiadomości.</param>
		/// <param name="subject">Temat wiadomości e-mail.</param>
		/// <param name="bodyHtml">Treść wiadomości w formacie HTML.</param>
		/// <returns>Obiekt <see cref="Task"/> reprezentujący asynchroniczną operację.</returns>
		public async Task SendAsync(string to, string subject, string bodyHtml)
		{
			// Utworzenie nowej wiadomości e-mail
			var msg = new MimeMessage();
			msg.From.Add(MailboxAddress.Parse(_cfg.From));
			msg.To.Add(MailboxAddress.Parse(to));
			msg.Subject = subject;
			msg.Body = new BodyBuilder { HtmlBody = bodyHtml }.ToMessageBody();

			// Nawiązanie połączenia z serwerem SMTP i wysłanie wiadomości
			using var smtp = new SmtpClient();
			await smtp.ConnectAsync(
				_cfg.Host, _cfg.Port,
				_cfg.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
			await smtp.AuthenticateAsync(_cfg.User, _cfg.Password);
			await smtp.SendAsync(msg);
			await smtp.DisconnectAsync(true);
		}
	}
}