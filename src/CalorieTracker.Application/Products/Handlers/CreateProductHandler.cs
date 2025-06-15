// Plik CreateProductHandler.cs - implementacja handlera tworzenia produktu.
// Odpowiada za proces tworzenia nowego produktu w systemie i zapis go w bazie danych.

using CalorieTracker.Application.Interfaces;
using CalorieTracker.Application.Products.Commands;
using CalorieTracker.Domain.Entities;

namespace CalorieTracker.Application.Auth.Handlers
{
	/// <summary>
	/// Handler odpowiedzialny za przetwarzanie komendy tworzenia nowego produktu.
	/// Mapuje dane z komendy na encję produktu i zapisuje ją w bazie danych.
	/// </summary>
	public class CreateProductHandler
	{
		/// <summary>
		/// Kontekst bazy danych do zarządzania produktami.
		/// </summary>
		private readonly IAppDbContext _db;

		/// <summary>
		/// Inicjalizuje nową instancję handlera tworzenia produktu.
		/// </summary>
		/// <param name="db">Kontekst bazy danych <see cref="IAppDbContext"/>.</param>
		public CreateProductHandler(IAppDbContext db) => _db = db;

		/// <summary>
		/// Główna metoda handlera wykonująca proces tworzenia produktu.
		/// </summary>
		/// <param name="command">Komenda <see cref="CreateProductCommand"/> zawierająca dane nowego produktu.</param>
		/// <returns>
		/// Task asynchroniczny zwracający identyfikator GUID nowo utworzonego produktu.
		/// </returns>
		public async Task<Guid> Handle(CreateProductCommand command)
		{
			// Tworzy nową encję produktu na podstawie danych z komendy
			var product = new Product
			{
				Name = command.Name,
				Brand = command.Brand,
				Description = command.Description,
				Ingredients = command.Ingredients,
				Barcode = command.Barcode,
				Category = command.Category,
				Unit = command.Unit,
				ServingSize = command.ServingSize,
				CaloriesPer100g = command.CaloriesPer100g,
				ProteinPer100g = command.ProteinPer100g,
				FatPer100g = command.FatPer100g,
				CarbohydratesPer100g = command.CarbohydratesPer100g,
				FiberPer100g = command.FiberPer100g,
				SugarsPer100g = command.SugarsPer100g,
				SodiumPer100g = command.SodiumPer100g,
				CreatedByUserId = command.CreatedByUserId,
				CreatedAt = DateTime.UtcNow,    // Automatyczne ustawienie daty utworzenia
				UpdatedAt = DateTime.UtcNow    // Automatyczne ustawienie daty modyfikacji
			};

			// Dodaje produkt do kontekstu śledzenia
			_db.Products.Add(product);

			// Zapisuje zmiany w bazie danych
			await _db.SaveChangesAsync();

			// Zwraca identyfikator nowo utworzonego produktu
			return product.Id;
		}
	}
}