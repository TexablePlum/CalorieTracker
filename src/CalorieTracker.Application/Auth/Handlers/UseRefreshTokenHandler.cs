using CalorieTracker.Application.Auth.Commands;
using CalorieTracker.Application.Auth.Interfaces;
using CalorieTracker.Application.Interfaces;
using CalorieTracker.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.Auth.Handlers;

public class UseRefreshTokenHandler
{
	private readonly IAppDbContext _db;
	private readonly IJwtGenerator _jwt;
	private readonly UserManager<ApplicationUser> _users;

	public UseRefreshTokenHandler(IAppDbContext db,
								  IJwtGenerator jwt,
								  UserManager<ApplicationUser> users) => (_db, _jwt, _users) = (db, jwt, users);

	/// <returns>null gdy refresh jest zły lub wygasły</returns>
	public async Task<(string? access, string? refresh)> Handle(UseRefreshTokenCommand cmd)
	{
		// Szukanie refresh tokena
		var rt = await _db.RefreshTokens
						  .FirstOrDefaultAsync(t => t.Token == cmd.RefreshToken);

		if (rt is null || rt.Revoked || rt.ExpiresAt < DateTime.UtcNow)
			return (null, null); // nieprawidłowy lub zużyty

		// Pobieranie użytkownika
		var user = await _users.FindByIdAsync(rt.UserId);
		if (user is null) return (null, null);

		// Rotacja: unieważnianie starego refresh tokena
		rt.Revoked = true;

		// Generowanie nowego refresh tokena
		var newRt = new RefreshToken
		{
			UserId = user.Id,
			ExpiresAt = DateTime.UtcNow.AddDays(7)
		};
		_db.RefreshTokens.Add(newRt);
		await _db.SaveChangesAsync();

		// Nowy access token
		var newAccess = _jwt.CreateToken(user);
		return (newAccess, newRt.Token);
	}
}
