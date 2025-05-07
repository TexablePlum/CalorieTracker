using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalorieTracker.Application.Auth.Interfaces
{
    public interface IEmailSender
    {
		/// <param name="to">Adres odbiorcy (np. user@example.com)</param>
		/// <param name="subject">Temat wiadomości</param>
		/// <param name="bodyHtml">Pełne ciało HTML</param>
		Task SendAsync(string to, string subject, string bodyHtml);
	}
}
