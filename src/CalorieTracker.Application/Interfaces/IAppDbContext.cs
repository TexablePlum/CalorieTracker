// Plik IAppDbContext.cs - definicja interfejsu kontekstu bazy danych aplikacji.
// Określa główne DbSety oraz operacje dostępu do danych dla całej aplikacji.

using CalorieTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.Interfaces;

/// <summary>
/// Główny interfejs kontekstu bazy danych aplikacji.
/// Definiuje wszystkie dostępne kolekcje encji oraz podstawowe operacje na bazie danych.
/// </summary>
public interface IAppDbContext
{
	/// <summary>
	/// Kolekcja tokenów odświeżających używanych w procesie uwierzytelniania.
	/// </summary>
	DbSet<RefreshToken> RefreshTokens { get; }

	/// <summary>
	/// Kolekcja kodów potwierdzających email użytkownika.
	/// </summary>
	DbSet<EmailConfirmation> EmailConfirmations { get; set; }

	/// <summary>
	/// Kolekcja kodów resetujących hasło użytkownika.
	/// </summary>
	DbSet<PasswordReset> PasswordResets { get; set; }

	/// <summary>
	/// Kolekcja profili użytkowników zawierających dodatkowe dane.
	/// </summary>
	DbSet<UserProfile> UserProfiles { get; set; }

	/// <summary>
	/// Kolekcja produktów dostępnych w aplikacji.
	/// </summary>
	DbSet<Product> Products { get; set; }

	/// <summary>
	/// Kolekcja przepisów stworzonych przez użytkowników.
	/// </summary>
	DbSet<Recipe> Recipes { get; set; }

	/// <summary>
	/// Kolekcja składników przepisów (tabela łącząca).
	/// </summary>
	DbSet<RecipeIngredient> RecipeIngredients { get; set; }

	/// <summary>
	/// Kolekcja pomiarów wagi użytkowników.
	/// </summary>
	DbSet<WeightMeasurement> WeightMeasurements { get; set; }

	/// <summary>
	/// Asynchronicznie zapisuje wszystkie zmiany w kontekście do bazy danych.
	/// </summary>
	/// <param name="ct">Token anulowania do bezpiecznego przerwania operacji.</param>
	/// <returns>Liczba zmodyfikowanych encji w bazie danych.</returns>
	Task<int> SaveChangesAsync(CancellationToken ct = default);
}