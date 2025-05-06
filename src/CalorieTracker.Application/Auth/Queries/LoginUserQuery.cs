namespace CalorieTracker.Application.Auth.Queries
{
    public class LoginUserQuery
    {
		public string Email { get; init; } = null!;
		public string Password { get; init; } = null!;
	}
}
