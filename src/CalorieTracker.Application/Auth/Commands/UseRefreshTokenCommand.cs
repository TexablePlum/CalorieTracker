// Plik UseRefreshTokenCommand.cs - definicja komendy użycia tokena odświeżającego.
// Odpowiada za przekazanie par tokenów wymaganych do procesu odświeżania autentykacji użytkownika.

namespace CalorieTracker.Application.Auth.Commands;

/// <summary>
/// Rekord reprezentujący komendę użycia tokena odświeżającego.
/// Zawiera zarówno aktualny token dostępowy jak i token odświeżający potrzebne do procesu odnowienia autentykacji.
/// </summary>
/// <param name="AccessToken">Aktualny token JWT użytkownika wymagający odświeżenia.</param>
/// <param name="RefreshToken">Token odświeżający używany do weryfikacji i generowania nowego tokena dostępowego.</param>
public record UseRefreshTokenCommand(string AccessToken, string RefreshToken);