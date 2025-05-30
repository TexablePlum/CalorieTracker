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
		public DbSet<Product> Products { get; set; } = null!;

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


			// Konfiguracja encji Product
			builder.Entity<Product>(entity =>
			{
				// Klucz główny
				entity.HasKey(p => p.Id);

				// Parsowanie enumów na stringi
				entity.Property(p => p.Category)
					.HasConversion<string>()
					.IsRequired();

				entity.Property(p => p.Unit)
					.HasConversion<string>()
					.IsRequired();

				// Indeksy dla lepszej wydajności wyszukiwania
				entity.HasIndex(p => p.Name);
				entity.HasIndex(p => p.Barcode);
				entity.HasIndex(p => p.Category);
				entity.HasIndex(p => p.CreatedByUserId);
				entity.HasIndex(p => p.IsVerified);

				// Relacja N:1 z ApplicationUser (produkty użytkownika)
				entity.HasOne(p => p.CreatedByUser)
					.WithMany()
					.HasForeignKey(p => p.CreatedByUserId)
					.OnDelete(DeleteBehavior.SetNull); // Po usunięciu użytkownika, produkty pozostają ale tracą właściciela

				// Ograniczenia długości stringów (już zdefiniowane w adnotacjach, ale dla pewności)
				entity.Property(p => p.Name).HasMaxLength(200);
				entity.Property(p => p.Brand).HasMaxLength(100);
				entity.Property(p => p.Description).HasMaxLength(1000);
				entity.Property(p => p.Ingredients).HasMaxLength(2000);
				entity.Property(p => p.Barcode).HasMaxLength(50);
			});

		}
	}
}
