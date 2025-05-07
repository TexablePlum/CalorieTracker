using CalorieTracker.Application.Auth.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace CalorieTracker.Infrastructure.Email
{
	public class EmailSender : IEmailSender
	{
		private readonly EmailSettings _cfg;
		public EmailSender(IOptions<EmailSettings> cfg) => _cfg = cfg.Value;

		public async Task SendAsync(string to, string subject, string bodyHtml)
		{
			var msg = new MimeMessage();
			msg.From.Add(MailboxAddress.Parse(_cfg.From));
			msg.To.Add(MailboxAddress.Parse(to));
			msg.Subject = subject;
			msg.Body = new BodyBuilder { HtmlBody = bodyHtml }.ToMessageBody();

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
