namespace CalorieTracker.Application.Auth.Commands;

public record UseRefreshTokenCommand(string AccessToken, string RefreshToken);
