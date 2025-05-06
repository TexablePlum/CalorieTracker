namespace CalorieTracker.Api.Models.Auth;

public class LogoutRequest
{
	public string RefreshToken { get; set; } = null!;
}
