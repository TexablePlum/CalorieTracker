// Plik EmailSettings.cs - klasa konfiguracyjna dla usługi poczty e-mail.
// Przechowuje parametry połączenia z serwerem SMTP oraz informacje o nadawcy wiadomości.
namespace CalorieTracker.Infrastructure.Email
{
	/// <summary>
	/// Klasa konfiguracyjna zawierająca ustawienia serwera poczty e-mail.
	/// Wartości właściwości są pobierane z pliku konfiguracyjnego appsettings.json.
	/// </summary>
	public class EmailSettings
	{
		/// <summary>
		/// Nazwa hosta lub adres IP serwera SMTP.
		/// </summary>
		public string Host { get; init; } = null!;

		/// <summary>
		/// Numer portu, na którym nasłuchuje serwer SMTP.
		/// Standardowo dla SMTP: 25, dla SMTP z SSL: 465, dla SMTP z TLS: 587.
		/// </summary>
		public int Port { get; init; }

		/// <summary>
		/// Nazwa użytkownika używana do uwierzytelniania na serwerze SMTP.
		/// </summary>
		public string User { get; init; } = null!;

		/// <summary>
		/// Hasło używane do uwierzytelniania na serwerze SMTP.
		/// </summary>
		public string Password { get; init; } = null!;

		/// <summary>
		/// Określa, czy połączenie z serwerem SMTP powinno być zabezpieczone protokołem SSL/TLS.
		/// </summary>
		public bool EnableSsl { get; init; }

		/// <summary>
		/// Adres e-mail, który będzie wyświetlany jako nadawca wiadomości.
		/// </summary>
		public string From { get; init; } = null!;
	}
}