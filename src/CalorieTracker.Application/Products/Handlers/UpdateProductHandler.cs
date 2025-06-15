// Plik UpdateProductHandler.cs - handler komendy aktualizacji produktu.
// Odpowiada za obsługę żądania aktualizacji danych produktu z uwzględnieniem kontroli uprawnień.

using CalorieTracker.Application.Auth.Commands;
using CalorieTracker.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.Auth.Handlers
{
	/// <summary>
	/// Handler implementujący logikę aktualizacji danych produktu.
	/// Zapewnia walidację uprawnień - tylko właściciel produktu może go modyfikować.
	/// </summary>
	public class UpdateProductHandler
	{
		/// <summary>
		/// Prywatne pole przechowujące instancję kontekstu bazy danych.
		/// </summary>
		private readonly IAppDbContext _db;

		/// <summary>
		/// Inicjalizuje nową instancję klasy <see cref="UpdateProductHandler"/>.
		/// </summary>
		/// <param name="db">Kontekst bazy danych implementujący <see cref="IAppDbContext"/>.</param>
		public UpdateProductHandler(IAppDbContext db) => _db = db;

		/// <summary>
		/// Asynchronicznie obsługuje komendę <see cref="UpdateProductCommand"/>.
		/// Aktualizuje dane produktu po uprzednim sprawdzeniu uprawnień użytkownika.
		/// </summary>
		/// <param name="command">Obiekt komendy <see cref="UpdateProductCommand"/> zawierający:
		/// <list type="bullet">
		/// <item><description>Id - identyfikator produktu do aktualizacji</description></item>
		/// <item><description>UpdatedByUserId - identyfikator użytkownika próbującego zaktualizować produkt</description></item>
		/// <item><description>Pozostałe pola - nowe wartości właściwości produktu</description></item>
		/// </list>
		/// </param>
		/// <returns>
		/// Zadanie zwracające <see cref="bool"/>:
		/// <list type="bullet">
		/// <item><description>true - jeśli aktualizacja się powiodła</description></item>
		/// <item><description>false - jeśli produkt nie istnieje lub użytkownik nie ma uprawnień</description></item>
		/// </list>
		/// </returns>
		public async Task<bool> Handle(UpdateProductCommand command)
		{
			// Pobranie produktu do aktualizacji
			var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == command.Id);
			if (product is null) return false; // Produkt nie istnieje

			// Sprawdzenie uprawnień - tylko właściciel może edytować produkt
			if (product.CreatedByUserId != command.UpdatedByUserId && !await IsAdmin(command.UpdatedByUserId))
				return false; // Brak uprawnień

			// Aktualizacja właściwości produktu
			product.Name = command.Name;
			product.Brand = command.Brand;
			product.Description = command.Description;
			product.Ingredients = command.Ingredients;
			product.Barcode = command.Barcode;
			product.Category = command.Category;
			product.Unit = command.Unit;
			product.ServingSize = command.ServingSize;
			product.CaloriesPer100g = command.CaloriesPer100g;
			product.ProteinPer100g = command.ProteinPer100g;
			product.FatPer100g = command.FatPer100g;
			product.CarbohydratesPer100g = command.CarbohydratesPer100g;
			product.FiberPer100g = command.FiberPer100g;
			product.SugarsPer100g = command.SugarsPer100g;
			product.SodiumPer100g = command.SodiumPer100g;
			product.UpdatedAt = DateTime.UtcNow; // Aktualizacja znacznika czasu

			// Zapis zmian w bazie danych
			await _db.SaveChangesAsync();
			return true; // Aktualizacja powiodła się
		}

		/// <summary>
		/// Sprawdza, czy użytkownik ma uprawnienia administratora.
		/// </summary>
		/// <param name="userId">Identyfikator użytkownika do sprawdzenia.</param>
		/// <returns>Zadanie zwracające <see cref="bool"/> określające czy użytkownik jest administratorem.</returns>
		/// <remarks>
		/// Obecnie funkcjonalność nie jest zaimplementowana - zawsze zwraca false.
		/// </remarks>
		private Task<bool> IsAdmin(string userId)
		{
			return Task.FromResult(false); // Tymczasowa implementacja
		}
	}
}