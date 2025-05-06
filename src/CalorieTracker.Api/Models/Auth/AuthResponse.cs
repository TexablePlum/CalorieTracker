namespace CalorieTracker.Api.Models.Auth;

public class AuthResponse
{
	public string AccessToken { get; init; } = null!;
	public string RefreshToken { get; init; } = null!;
	public DateTime ExpiresAt { get; init; }
}
