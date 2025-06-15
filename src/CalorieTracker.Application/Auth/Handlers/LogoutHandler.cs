// Plik LogoutHandler.cs - implementacja handlera wylogowania użytkownika.
// Odpowiada za unieważnienie tokena odświeżającego podczas procesu wylogowania.

using CalorieTracker.Application.Auth.Commands;
using CalorieTracker.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.Auth.Handlers;

/// <summary>
/// Handler odpowiedzialny za proces wylogowania użytkownika.
/// Unieważnia token odświeżający, uniemożliwiając jego ponowne użycie do uwierzytelniania.
/// </summary>
public class LogoutHandler
{
	/// <summary>
	/// Kontekst bazy danych do zarządzania tokenami odświeżającymi.
	/// </summary>
	private readonly IAppDbContext _db;

	/// <summary>
	/// Inicjalizuje nową instancję handlera wylogowania.
	/// </summary>
	/// <param name="db">Kontekst bazy danych <see cref="IAppDbContext"/>.</param>
	public LogoutHandler(IAppDbContext db) => _db = db;

	/// <summary>
	/// Główna metoda handlera wykonująca unieważnienie tokena odświeżającego.
	/// </summary>
	/// <param name="cmd">Komenda <see cref="LogoutCommand"/> zawierająca token do unieważnienia.</param>
	/// <returns>Task reprezentujący operację asynchroniczną.</returns>
	public async Task Handle(LogoutCommand cmd)
	{
		// Wyszukuje token odświeżający w bazie danych
		var rt = await _db.RefreshTokens
						  .FirstOrDefaultAsync(t => t.Token == cmd.RefreshToken);

		// Jeśli token nie istnieje, kończy działanie
		if (rt is null) return;

		// Oznacza token jako unieważniony
		rt.Revoked = true;

		// Zapisuje zmiany w bazie danych
		await _db.SaveChangesAsync();
	}
}