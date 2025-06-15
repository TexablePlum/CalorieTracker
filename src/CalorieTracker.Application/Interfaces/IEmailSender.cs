// Plik IEmailSender.cs - definicja interfejsu serwisu wysyłki wiadomości email.
// Określa kontrakt dla komponentu odpowiedzialnego za wysyłkę elektronicznej korespondencji systemowej.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalorieTracker.Application.Auth.Interfaces
{
	/// <summary>
	/// Interfejs serwisu odpowiedzialnego za wysyłkę wiadomości email.
	/// Definiuje podstawową operację wysyłki wiadomości w systemie.
	/// </summary>
	public interface IEmailSender
	{
		/// <summary>
		/// Wysyła asynchronicznie wiadomość email do wskazanego odbiorcy.
		/// </summary>
		/// <param name="to">Adres email odbiorcy w formacie zgodnym z RFC 5322 (np. user@example.com).</param>
		/// <param name="subject">Temat wiadomości email. Powinien być zwięzły i opisowy.</param>
		/// <param name="bodyHtml">Treść wiadomości w formacie HTML. Obsługuje pełne znaczniki HTML.</param>
		/// <returns>Task reprezentujący operację asynchroniczną.</returns>
		
		Task SendAsync(string to, string subject, string bodyHtml);
	}
}