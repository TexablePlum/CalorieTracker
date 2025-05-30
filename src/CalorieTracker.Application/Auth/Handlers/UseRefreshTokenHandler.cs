using CalorieTracker.Application.Auth.Commands;
using CalorieTracker.Application.Auth.Interfaces;
using CalorieTracker.Application.Interfaces;
using CalorieTracker.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace CalorieTracker.Application.Auth.Handlers
{
	public class UseRefreshTokenHandler
	{
		private readonly IAppDbContext _db;
		private readonly IJwtGenerator _jwt;
		private readonly UserManager<ApplicationUser> _users;

		public UseRefreshTokenHandler(IAppDbContext db, IJwtGenerator jwt, UserManager<ApplicationUser> users)
			=> (_db, _jwt, _users) = (db, jwt, users);

		/// <summary>
		/// Obsługuje refresh token z walidacją access tokenu
		/// </summary>
		/// <param name="cmd">Komenda zawierająca access i refresh token</param>
		/// <returns>Nowe tokeny lub null gdy walidacja nie przejdzie</returns>
		public async Task<(string? access, string? refresh)> Handle(UseRefreshTokenCommand cmd)
		{
			// 1. Sprawdzenie refresh tokenu (podstawowa walidacja)
			var rt = await _db.RefreshTokens
							  .FirstOrDefaultAsync(t => t.Token == cmd.RefreshToken);

			if (rt is null || rt.Revoked || rt.ExpiresAt < DateTime.UtcNow)
				return (null, null); // nieprawidłowy lub wygasły refresh token

			// 2. Walidacja access tokenu - czy należy do tego samego użytkownika
			var userIdFromAccessToken = ExtractUserIdFromAccessToken(cmd.AccessToken);
			if (userIdFromAccessToken is null || userIdFromAccessToken != rt.UserId)
				return (null, null); // access token nie należy do właściciela refresh tokenu

			// 3. Pobieranie użytkownika
			var user = await _users.FindByIdAsync(rt.UserId);
			if (user is null) return (null, null);

			// 4. Rotacja: unieważnianie starego refresh tokena
			rt.Revoked = true;

			// 5. Generowanie nowego refresh tokena
			var newRt = new RefreshToken
			{
				UserId = user.Id,
				ExpiresAt = DateTime.UtcNow.AddDays(7)
			};
			_db.RefreshTokens.Add(newRt);
			await _db.SaveChangesAsync();

			// 6. Usuwanie starych tokenów dla tego użytkownika (wygasłe i odwołane)
			var expiredRevoked = await _db.RefreshTokens
				.Where(t => t.UserId == user.Id && t.Revoked && t.ExpiresAt < DateTime.UtcNow)
				.ToListAsync();

			if (expiredRevoked.Any())
			{
				_db.RefreshTokens.RemoveRange(expiredRevoked);
				await _db.SaveChangesAsync();
			}

			// 7. Nowy access token
			var newAccess = _jwt.CreateToken(user);
			return (newAccess, newRt.Token);
		}

		/// <summary>
		/// Ekstraktuje User ID z access tokenu (obsługuje też wygasłe tokeny)
		/// </summary>
		/// <param name="accessToken">JWT access token</param>
		/// <returns>User ID lub null jeśli token jest nieprawidłowy</returns>
		private string? ExtractUserIdFromAccessToken(string accessToken)
		{
			try
			{
				// Parsowanie JWT bez walidacji sygnatury i czasu ważności
				var handler = new JwtSecurityTokenHandler();

				// Sprawdzenie czy to w ogóle jest poprawny format JWT
				if (!handler.CanReadToken(accessToken))
					return null;

				// Odczytanie tokenu (bez walidacji - pozwalamy na wygasłe)
				var jsonToken = handler.ReadJwtToken(accessToken);

				// Wyciągnięcie User ID z claim 'sub' (Subject)
				var userIdClaim = jsonToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub);
				return userIdClaim?.Value;
			}
			catch (Exception)
			{
				// Jeśli cokolwiek pójdzie nie tak (malformed token, etc.)
				return null;
			}
		}
	}
}