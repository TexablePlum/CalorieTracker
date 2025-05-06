using CalorieTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.Interfaces;

public interface IAppDbContext
{
	DbSet<RefreshToken> RefreshTokens { get; }

	Task<int> SaveChangesAsync(CancellationToken ct = default);
}
