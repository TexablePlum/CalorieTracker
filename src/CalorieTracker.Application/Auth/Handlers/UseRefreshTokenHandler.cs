// Plik UseRefreshTokenHandler.cs - implementacja handlera odświeżania tokenów.
// Odpowiada za proces rotacji tokenów uwierzytelniających przy użyciu refresh tokena.

using CalorieTracker.Application.Auth.Commands;
using CalorieTracker.Application.Auth.Interfaces;
using CalorieTracker.Application.Interfaces;
using CalorieTracker.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace CalorieTracker.Application.Auth.Handlers
{
	/// <summary>
	/// Handler odpowiedzialny za proces odświeżania tokenów uwierzytelniających.
	/// Wykonuje pełną walidację tokenów i generuje nową parę tokenów (access + refresh).
	/// </summary>
	public class UseRefreshTokenHandler
	{
		/// <summary>
		/// Kontekst bazy danych do zarządzania refresh tokenami.
		/// </summary>
		private readonly IAppDbContext _db;

		/// <summary>
		/// Generator tokenów JWT.
		/// </summary>
		private readonly IJwtGenerator _jwt;

		/// <summary>
		/// Menadżer użytkowników do weryfikacji danych konta.
		/// </summary>
		private readonly UserManager<ApplicationUser> _users;

		/// <summary>
		/// Inicjalizuje nową instancję handlera odświeżania tokenów.
		/// </summary>
		/// <param name="db">Kontekst bazy danych <see cref="IAppDbContext"/>.</param>
		/// <param name="jwt">Generator tokenów JWT <see cref="IJwtGenerator"/>.</param>
		/// <param name="users">Menadżer użytkowników <see cref="UserManager{ApplicationUser}"/>.</param>
		public UseRefreshTokenHandler(IAppDbContext db, IJwtGenerator jwt, UserManager<ApplicationUser> users)
			=> (_db, _jwt, _users) = (db, jwt, users);

		/// <summary>
		/// Główna metoda handlera wykonująca proces odświeżania tokenów.
		/// </summary>
		/// <param name="cmd">Komenda <see cref="UseRefreshTokenCommand"/> zawierająca tokeny.</param>
		/// <returns>
		/// Krotkę zawierającą nowy access token i refresh token, gdy walidacja przebiegnie pomyślnie;
		/// null w przypadku niepowodzenia walidacji.
		/// </returns>
		public async Task<(string? access, string? refresh)> Handle(UseRefreshTokenCommand cmd)
		{
			// 1. Podstawowa walidacja refresh tokenu
			var rt = await _db.RefreshTokens
							.FirstOrDefaultAsync(t => t.Token == cmd.RefreshToken);

			if (rt is null || rt.Revoked || rt.ExpiresAt < DateTime.UtcNow)
				return (null, null); // Nieprawidłowy lub wygasły refresh token

			// 2. Weryfikacja zgodności tokenów
			var userIdFromAccessToken = ExtractUserIdFromAccessToken(cmd.AccessToken);
			if (userIdFromAccessToken is null || userIdFromAccessToken != rt.UserId)
				return (null, null); // Tokeny należą do różnych użytkowników

			// 3. Pobranie danych użytkownika
			var user = await _users.FindByIdAsync(rt.UserId);
			if (user is null) return (null, null);

			// 4. Unieważnienie starego refresh tokena (rotacja tokenów)
			rt.Revoked = true;

			// 5. Generowanie nowego refresh tokena
			var newRt = new RefreshToken
			{
				UserId = user.Id,
				ExpiresAt = DateTime.UtcNow.AddDays(7) // Token ważny przez 7 dni
			};
			_db.RefreshTokens.Add(newRt);

			// 6. Czyszczenie starych, wygasłych tokenów
			var expiredRevoked = await _db.RefreshTokens
				.Where(t => t.UserId == user.Id && t.Revoked && t.ExpiresAt < DateTime.UtcNow)
				.ToListAsync();

			if (expiredRevoked.Any())
			{
				_db.RefreshTokens.RemoveRange(expiredRevoked);
			}

			await _db.SaveChangesAsync();

			// 7. Generowanie nowego access tokena
			var newAccess = _jwt.CreateToken(user);
			return (newAccess, newRt.Token);
		}

		/// <summary>
		/// Metoda pomocnicza wyciągająca identyfikator użytkownika z tokena JWT.
		/// </summary>
		/// <param name="accessToken">Token JWT w postaci ciągu znaków.</param>
		/// <returns>
		/// Identyfikator użytkownika (claim 'sub') lub null, jeśli token jest nieprawidłowy.
		/// </returns>
		private string? ExtractUserIdFromAccessToken(string accessToken)
		{
			try
			{
				// Inicjalizacja handlera tokenów JWT
				var handler = new JwtSecurityTokenHandler();

				// Sprawdzenie czy token ma poprawny format JWT
				if (!handler.CanReadToken(accessToken))
					return null;

				// Odczytanie tokenu bez walidacji (działa nawet z wygasłymi tokenami)
				var jsonToken = handler.ReadJwtToken(accessToken);

				// Pobranie claimu 'sub' (Subject) zawierającego ID użytkownika
				var userIdClaim = jsonToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub);
				return userIdClaim?.Value;
			}
			catch (Exception)
			{
				// Obsługa błędów parsowania tokenu
				return null;
			}
		}
	}
}