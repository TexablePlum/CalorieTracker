// Plik AppDbContext.cs - implementacja kontekstu bazy danych aplikacji.
// Odpowiada za mapowanie encji domenowych na struktury bazy danych oraz definiowanie relacji między nimi.
using CalorieTracker.Application.Interfaces;
using CalorieTracker.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Infrastructure.Data
{
	/// <summary>
	/// Implementacja interfejsu <see cref="IAppDbContext"/> odpowiedzialna za dostęp do bazy danych.
	/// Dziedziczy po <see cref="IdentityDbContext{TUser}"/> w celu wykorzystania mechanizmów uwierzytelniania ASP.NET Identity.
	/// </summary>
	public class AppDbContext : IdentityDbContext<ApplicationUser>, IAppDbContext
	{
		/// <summary>
		/// Inicjalizuje nową instancję klasy <see cref="AppDbContext"/>.
		/// </summary>
		/// <param name="options">Opcje konfiguracji kontekstu bazy danych.</param>
		public AppDbContext(DbContextOptions<AppDbContext> options)
			: base(options)
		{ }

		/// <summary>
		/// Kolekcja tokenów odświeżających dla użytkowników aplikacji.
		/// </summary>
		public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

		/// <summary>
		/// Kolekcja profili użytkowników zawierających dane osobowe i preferencje.
		/// </summary>
		public DbSet<UserProfile> UserProfiles { get; set; } = null!;

		/// <summary>
		/// Kolekcja potwierdzeń adresów e-mail wykorzystywanych w procesie rejestracji.
		/// </summary>
		public DbSet<EmailConfirmation> EmailConfirmations { get; set; } = null!;

		/// <summary>
		/// Kolekcja żądań resetowania haseł.
		/// </summary>
		public DbSet<PasswordReset> PasswordResets { get; set; } = null!;

		/// <summary>
		/// Kolekcja produktów spożywczych dostępnych w aplikacji.
		/// </summary>
		public DbSet<Product> Products { get; set; } = null!;

		/// <summary>
		/// Kolekcja przepisów kulinarnych tworzonych przez użytkowników.
		/// </summary>
		public DbSet<Recipe> Recipes { get; set; } = null!;

		/// <summary>
		/// Kolekcja składników wchodzących w skład przepisów.
		/// </summary>
		public DbSet<RecipeIngredient> RecipeIngredients { get; set; } = null!;

		/// <summary>
		/// Kolekcja pomiarów wagi użytkowników.
		/// </summary>
		public DbSet<WeightMeasurement> WeightMeasurements { get; set; } = null!;

		/// <summary>
		/// Konfiguruje model bazy danych przy jej tworzeniu.
		/// Definiuje relacje między encjami, indeksy oraz ograniczenia.
		/// </summary>
		/// <param name="builder">Obiekt <see cref="ModelBuilder"/> służący do konfiguracji modelu.</param>
		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			ConfigureUserProfile(builder);
			ConfigureProducts(builder);
			ConfigureRecipes(builder);
			ConfigureWeightMeasurements(builder);
		}

		/// <summary>
		/// Konfiguruje relacje i właściwości dla encji UserProfile.
		/// </summary>
		/// <param name="builder">Obiekt <see cref="ModelBuilder"/> służący do konfiguracji modelu.</param>
		private void ConfigureUserProfile(ModelBuilder builder)
		{
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

		/// <summary>
		/// Konfiguruje relacje, indeksy i ograniczenia dla encji Product.
		/// </summary>
		/// <param name="builder">Obiekt <see cref="ModelBuilder"/> służący do konfiguracji modelu.</param>
		private void ConfigureProducts(ModelBuilder builder)
		{
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

		/// <summary>
		/// Konfiguruje relacje, indeksy i ograniczenia dla encji Recipe i RecipeIngredient.
		/// </summary>
		/// <param name="builder">Obiekt <see cref="ModelBuilder"/> służący do konfiguracji modelu.</param>
		private void ConfigureRecipes(ModelBuilder builder)
		{
			// Konfiguracja encji Recipe
			builder.Entity<Recipe>(entity =>
			{
				// Klucz główny
				entity.HasKey(r => r.Id);

				// Indeksy dla lepszej wydajności
				entity.HasIndex(r => r.Name);
				entity.HasIndex(r => r.CreatedByUserId);
				entity.HasIndex(r => r.CreatedAt);

				// Relacja N:1 z ApplicationUser
				entity.HasOne(r => r.CreatedByUser)
					.WithMany()
					.HasForeignKey(r => r.CreatedByUserId)
					.OnDelete(DeleteBehavior.Cascade);

				// Relacja 1:N z RecipeIngredient
				entity.HasMany(r => r.Ingredients)
					.WithOne(i => i.Recipe)
					.HasForeignKey(i => i.RecipeId)
					.OnDelete(DeleteBehavior.Cascade);

				// Ograniczenia długości stringów
				entity.Property(r => r.Name).HasMaxLength(200);
				entity.Property(r => r.Instructions).HasMaxLength(5000);
			});

			// Konfiguracja encji RecipeIngredient
			builder.Entity<RecipeIngredient>(entity =>
			{
				// Klucz główny
				entity.HasKey(i => i.Id);

				// Indeksy
				entity.HasIndex(i => i.RecipeId);
				entity.HasIndex(i => i.ProductId);

				// Relacja N:1 z Recipe (już skonfigurowana wyżej)

				// Relacja N:1 z Product
				entity.HasOne(i => i.Product)
					.WithMany()
					.HasForeignKey(i => i.ProductId)
					.OnDelete(DeleteBehavior.Restrict); // Nie usuwaj produktu jeśli jest używany w przepisach
			});
		}

		/// <summary>
		/// Konfiguruje relacje, indeksy i ograniczenia dla encji WeightMeasurement.
		/// </summary>
		/// <param name="builder">Obiekt <see cref="ModelBuilder"/> służący do konfiguracji modelu.</param>
		private void ConfigureWeightMeasurements(ModelBuilder builder)
		{
			// Konfiguracja encji WeightMeasurement
			builder.Entity<WeightMeasurement>(entity =>
			{
				// Klucz główny
				entity.HasKey(w => w.Id);

				// Indeksy dla wydajności
				entity.HasIndex(w => w.UserId);
				entity.HasIndex(w => w.MeasurementDate);
				entity.HasIndex(w => new { w.UserId, w.MeasurementDate }).IsUnique(); // Jeden pomiar na dzień

				// Relacja N:1 z ApplicationUser
				entity.HasOne(w => w.User)
					.WithMany()
					.HasForeignKey(w => w.UserId)
					.OnDelete(DeleteBehavior.Cascade);

				// Konwersja DateOnly (jeśli potrzebne)
				entity.Property(w => w.MeasurementDate)
					.HasConversion<DateOnly>();
			});
		}
	}
}