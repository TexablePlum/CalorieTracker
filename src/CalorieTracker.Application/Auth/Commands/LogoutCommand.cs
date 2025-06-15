// Plik LogoutCommand.cs - definicja komendy wylogowania użytkownika.
// Odpowiada za przekazanie tokena odświeżającego, który ma zostać unieważniony podczas procesu wylogowania.

namespace CalorieTracker.Application.Auth.Commands;

/// <summary>
/// Rekord reprezentujący komendę wylogowania użytkownika.
/// Zawiera token odświeżający, który powinien zostać unieważniony w systemie.
/// </summary>
/// <param name="RefreshToken">Token odświeżający, który ma zostać unieważniony podczas wylogowania.</param>
public record LogoutCommand(string RefreshToken);