namespace CalorieTracker.Api.Models.Auth
{
	public class AuthResponse
	{
		public string Token { get; init; } = null!;
		public DateTime ExpiresAt { get; init; }
	}
}
