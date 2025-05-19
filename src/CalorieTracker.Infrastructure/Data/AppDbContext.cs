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
		public DbSet<UserProfile> UserProfiles { get; set; } = null!;
		public DbSet<EmailConfirmation> EmailConfirmations { get; set; } = null!;
		public DbSet<PasswordReset> PasswordResets { get; set; } = null!;

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			// Relacja 1:1 ApplicationUser - UserProfile
			builder.Entity<ApplicationUser>()
				.HasOne(u => u.Profile)
				.WithOne(p => p.User)
				.HasForeignKey<UserProfile>(p => p.UserId);

			// Parsowanie enumów na stringi
			builder.Entity<UserProfile>()
				.Property(p => p.Gender)
				.HasConversion<string>();

			builder.Entity<UserProfile>()
				.Property(p => p.ActivityLevel)
				.HasConversion<string>();

			builder.Entity<UserProfile>()
				.Property(p => p.Goal)
				.HasConversion<string>();

		}
	}
}
