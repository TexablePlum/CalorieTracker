using CalorieTracker.Application.Auth.Commands;
using CalorieTracker.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.Auth.Handlers;

public class LogoutHandler
{
	private readonly IAppDbContext _db;
	public LogoutHandler(IAppDbContext db) => _db = db;

	public async Task Handle(LogoutCommand cmd)
	{
		var rt = await _db.RefreshTokens
						  .FirstOrDefaultAsync(t => t.Token == cmd.RefreshToken);

		if (rt is null) return;

		rt.Revoked = true;
		await _db.SaveChangesAsync();
	}
}
