using CalorieTracker.Application.Interfaces;
using CalorieTracker.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>, IAppDbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options)
			: base(options)
		{}

		public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
	}
}
