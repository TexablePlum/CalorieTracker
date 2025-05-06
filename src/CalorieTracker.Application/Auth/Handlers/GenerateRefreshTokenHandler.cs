using CalorieTracker.Application.Auth.Commands;
using CalorieTracker.Application.Interfaces;
using CalorieTracker.Domain.Entities;

namespace CalorieTracker.Application.Auth.Handlers;

public class GenerateRefreshTokenHandler
{
	private readonly IAppDbContext _db;
	public GenerateRefreshTokenHandler(IAppDbContext db) => _db = db;

	public async Task<string> Handle(GenerateRefreshTokenCommand cmd)
	{
		var rt = new RefreshToken
		{
			UserId = cmd.UserId,
			ExpiresAt = DateTime.UtcNow.AddDays(7)
		};
		_db.RefreshTokens.Add(rt);
		await _db.SaveChangesAsync();
		return rt.Token;
	}
}
