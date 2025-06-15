// Plik GenerateRefreshTokenHandler.cs - implementacja handlera generowania tokena odświeżającego.
// Odpowiada za tworzenie i przechowywanie nowego tokena odświeżającego dla użytkownika.

using CalorieTracker.Application.Auth.Commands;
using CalorieTracker.Application.Interfaces;
using CalorieTracker.Domain.Entities;

namespace CalorieTracker.Application.Auth.Handlers;

/// <summary>
/// Handler odpowiedzialny za generowanie nowego tokena odświeżającego.
/// Tworzy i zapisuje token w bazie danych z domyślnym czasem ważności 7 dni.
/// </summary>
public class GenerateRefreshTokenHandler
{
	/// <summary>
	/// Kontekst bazy danych do zarządzania tokenami odświeżającymi.
	/// </summary>
	private readonly IAppDbContext _db;

	/// <summary>
	/// Inicjalizuje nową instancję handlera generowania tokena odświeżającego.
	/// </summary>
	/// <param name="db">Kontekst bazy danych <see cref="IAppDbContext"/>.</param>
	public GenerateRefreshTokenHandler(IAppDbContext db) => _db = db;

	/// <summary>
	/// Główna metoda handlera tworząca nowy token odświeżający.
	/// </summary>
	/// <param name="cmd">Komenda <see cref="GenerateRefreshTokenCommand"/> zawierająca identyfikator użytkownika.</param>
	/// <returns>Task asynchroniczny zwracający wygenerowany token jako ciąg znaków.</returns>
	public async Task<string> Handle(GenerateRefreshTokenCommand cmd)
	{
		// Tworzy nowy token odświeżający z ważnością 7 dni
		var rt = new RefreshToken
		{
			UserId = cmd.UserId,
			ExpiresAt = DateTime.UtcNow.AddDays(7)
		};

		// Zapisuje token w bazie danych
		_db.RefreshTokens.Add(rt);
		await _db.SaveChangesAsync();

		// Zwraca wygenerowany token
		return rt.Token;
	}
}