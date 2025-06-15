// Plik GenerateRefreshTokenCommand.cs - definicja komendy generowania tokena odświeżającego.
// Odpowiada za przekazanie danych potrzebnych do wygenerowania nowego tokena odświeżającego dla użytkownika.

namespace CalorieTracker.Application.Auth.Commands;

/// <summary>
/// Rekord reprezentujący komendę generowania tokena odświeżającego.
/// Przenosi identyfikator użytkownika, dla którego ma zostać wygenerowany nowy token.
/// </summary>
/// <param name="UserId">Identyfikator użytkownika, dla którego generowany jest token.</param>
public record GenerateRefreshTokenCommand(string UserId);