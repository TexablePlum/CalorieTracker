// Plik GetUserProductsHandler.cs - handler zapytania o pobranie produktów użytkownika.
// Odpowiada za obsługę żądania pobrania listy produktów przypisanych do konkretnego użytkownika.

using CalorieTracker.Application.Auth.Queries;
using CalorieTracker.Application.Interfaces;
using CalorieTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.Auth.Handlers
{
	/// <summary>
	/// Handler implementujący logikę pobierania produktów powiązanych z określonym użytkownikiem.
	/// Wykorzystuje kontekst bazy danych do wykonania stronicowanego zapytania Entity Framework Core.
	/// </summary>
	public class GetUserProductsHandler
	{
		/// <summary>
		/// Prywatne pole przechowujące instancję kontekstu bazy danych.
		/// </summary>
		private readonly IAppDbContext _db;

		/// <summary>
		/// Inicjalizuje nową instancję klasy <see cref="GetUserProductsHandler"/>.
		/// </summary>
		/// <param name="db">Kontekst bazy danych implementujący <see cref="IAppDbContext"/>.</param>
		public GetUserProductsHandler(IAppDbContext db) => _db = db;

		/// <summary>
		/// Asynchronicznie obsługuje zapytanie <see cref="GetUserProductsQuery"/>.
		/// Pobiera paginowaną listę produktów utworzonych przez określonego użytkownika,
		/// posortowaną malejąco według daty utworzenia.
		/// </summary>
		/// <param name="query">Obiekt zapytania <see cref="GetUserProductsQuery"/> zawierający:
		/// <list type="bullet">
		/// <item><description>UserId - identyfikator użytkownika</description></item>
		/// <item><description>Skip - ilość elementów do pominięcia</description></item>
		/// <item><description>Take - ilość elementów do pobrania</description></item>
		/// </list>
		/// </param>
		/// <returns>
		/// Zadanie zwracające listę <see cref="List{Product}"/> zawierającą produkty użytkownika
		/// lub pustą listę, jeśli użytkownik nie ma przypisanych produktów.
		/// </returns>
		public async Task<List<Product>> Handle(GetUserProductsQuery query)
		{
			return await _db.Products
				.Where(p => p.CreatedByUserId == query.UserId)	// Filtrowanie po ID użytkownika
				.OrderByDescending(p => p.CreatedAt)			// Sortowanie od najnowszych
				.Skip(query.Skip)								// Pominięcie określonej liczby rekordów
				.Take(query.Take)								// Pobranie określonej liczby rekordów
				.ToListAsync();									// Konwersja na listę
		}
	}
}