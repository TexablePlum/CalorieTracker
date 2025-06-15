// Plik AuthResponse.cs - model odpowiedzi uwierzytelniającej.
// Definiuje strukturę danych zwracaną po pomyślnym uwierzytelnieniu użytkownika.

namespace CalorieTracker.Api.Models.Auth;

/// <summary>
/// Model odpowiedzi zawierający dane uwierzytelniające użytkownika.
/// Używany do przekazania tokenów dostępu i odświeżania po pomyślnym zalogowaniu.
/// </summary>
public class AuthResponse
{
	/// <summary>
	/// Token JWT używany do uwierzytelniania żądań.
	/// Wymagany do autoryzacji dostępu do chronionych zasobów.
	/// </summary>
	public string AccessToken { get; init; } = null!;

	/// <summary>
	/// Token służący do odświeżania sesji użytkownika.
	/// Pozwala na uzyskanie nowego tokenu dostępu po wygaśnięciu obecnego.
	/// </summary>
	public string RefreshToken { get; init; } = null!;

	/// <summary>
	/// Data i czas wygaśnięcia tokenu dostępu.
	/// Określa do kiedy token jest ważny i może być używany.
	/// </summary>
	public DateTime ExpiresAt { get; init; }
}