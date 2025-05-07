namespace CalorieTracker.Infrastructure.Email
{
	/// <summary> Ustawienia odczytywane z appsettings </summary>
	public class EmailSettings
    {
		public string Host { get; init; } = null!;
		public int Port { get; init; }
		public string User { get; init; } = null!;
		public string Password { get; init; } = null!;
		public bool EnableSsl { get; init; }
		public string From { get; init; } = null!;
		public string ConfirmUrl { get; init; } = null!;
	}
}
