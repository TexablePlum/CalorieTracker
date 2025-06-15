// Plik GetProductByIdHandler.cs - handler zapytania o pobranie produktu po identyfikatorze.
// Odpowiada za obsługę żądania pobrania szczegółów produktu z bazy danych na podstawie ID.

using CalorieTracker.Application.Auth.Queries;
using CalorieTracker.Application.Interfaces;
using CalorieTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.Auth.Handlers
{
	/// <summary>
	/// Handler implementujący logikę pobierania produktu z bazy danych na podstawie identyfikatora.
	/// Wykonuje wyszukiwanie produktu w bazie danych i zwraca jego pełne dane.
	/// </summary>
	public class GetProductByIdHandler
	{
		/// <summary>
		/// Prywatne pole przechowujące instancję kontekstu bazy danych.
		/// </summary>
		private readonly IAppDbContext _db;

		/// <summary>
		/// Inicjalizuje nową instancję klasy <see cref="GetProductByIdHandler"/>.
		/// </summary>
		/// <param name="db">Kontekst bazy danych implementujący <see cref="IAppDbContext"/>.</param>
		public GetProductByIdHandler(IAppDbContext db) => _db = db;

		/// <summary>
		/// Asynchronicznie obsługuje zapytanie <see cref="GetProductByIdQuery"/>.
		/// Pobiera produkt z bazy danych wraz z danymi użytkownika, który go utworzył.
		/// </summary>
		/// <param name="query">Obiekt zapytania <see cref="GetProductByIdQuery"/> zawierający identyfikator produktu.</param>
		/// <returns>
		/// Zadanie zwracające obiekt <see cref="Product"/> lub wartość null, 
		/// jeśli produkt o podanym ID nie istnieje w bazie danych.
		/// </returns>
		public async Task<Product?> Handle(GetProductByIdQuery query)
		{
			return await _db.Products
				.Include(p => p.CreatedByUser) // Dołączenie danych użytkownika tworzącego produkt
				.FirstOrDefaultAsync(p => p.Id == query.Id); // Wyszukanie produktu po ID
		}
	}
}