using CalorieTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.Interfaces;

public interface IAppDbContext
{
	DbSet<RefreshToken> RefreshTokens { get; }
	DbSet<EmailConfirmation> EmailConfirmations { get; set; }
	DbSet<PasswordReset> PasswordResets { get; set; }

	Task<int> SaveChangesAsync(CancellationToken ct = default);
}
