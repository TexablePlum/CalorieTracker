// Plik SearchProductsHandler.cs - handler zapytania o wyszukiwanie produktów.
// Odpowiada za obsługę żądania wyszukiwania produktów z możliwością filtrowania i stronicowania.

using CalorieTracker.Application.Auth.Queries;
using CalorieTracker.Application.Interfaces;
using CalorieTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.Auth.Handlers
{
	/// <summary>
	/// Handler implementujący logikę wyszukiwania produktów z zastosowaniem różnych filtrów.
	/// Umożliwia wyszukiwanie po nazwie, marce, kategorii oraz statusie weryfikacji.
	/// </summary>
	public class SearchProductsHandler
	{
		/// <summary>
		/// Prywatne pole przechowujące instancję kontekstu bazy danych.
		/// </summary>
		private readonly IAppDbContext _db;

		/// <summary>
		/// Inicjalizuje nową instancję klasy <see cref="SearchProductsHandler"/>.
		/// </summary>
		/// <param name="db">Kontekst bazy danych implementujący <see cref="IAppDbContext"/>.</param>
		public SearchProductsHandler(IAppDbContext db) => _db = db;

		/// <summary>
		/// Asynchronicznie obsługuje zapytanie <see cref="SearchProductsQuery"/>.
		/// Wykonuje wyszukiwanie produktów z zastosowaniem podanych filtrów i paginacji.
		/// </summary>
		/// <param name="query">Obiekt zapytania <see cref="SearchProductsQuery"/> zawierający:
		/// <list type="bullet">
		/// <item><description>SearchTerm - fraza do wyszukania w nazwie i marce produktu</description></item>
		/// <item><description>Category - opcjonalny filtr kategorii produktu</description></item>
		/// <item><description>VerifiedOnly - flaga określająca czy zwracać tylko zweryfikowane produkty</description></item>
		/// <item><description>Skip - ilość elementów do pominięcia</description></item>
		/// <item><description>Take - ilość elementów do pobrania</description></item>
		/// </list>
		/// </param>
		/// <returns>
		/// Zadanie zwracające listę <see cref="List{Product}"/> zawierającą produkty spełniające kryteria wyszukiwania
		/// lub pustą listę, jeśli nie znaleziono pasujących produktów.
		/// </returns>
		public async Task<List<Product>> Handle(SearchProductsQuery query)
		{
			// Bazowe zapytanie do produktów
			var searchQuery = _db.Products.AsQueryable();

			// Filtrowanie po nazwie lub marce (wielkość liter nie ma znaczenia)
			if (!string.IsNullOrWhiteSpace(query.SearchTerm))
			{
				var searchTerm = query.SearchTerm.ToLower();
				searchQuery = searchQuery.Where(p =>
					p.Name.ToLower().Contains(searchTerm) || // Wyszukiwanie w nazwie produktu
					(p.Brand != null && p.Brand.ToLower().Contains(searchTerm))); // Wyszukiwanie w marce produktu
			}

			// Filtrowanie po kategorii (jeśli określona)
			if (query.Category.HasValue)
			{
				searchQuery = searchQuery.Where(p => p.Category == query.Category.Value);
			}

			// Filtrowanie tylko zweryfikowanych produktów (jeśli wymagane)
			if (query.VerifiedOnly)
			{
				searchQuery = searchQuery.Where(p => p.IsVerified);
			}

			// Wykonanie zapytania z sortowaniem i paginacją
			return await searchQuery
				.OrderBy(p => p.Name)	// Sortowanie alfabetyczne po nazwie
				.Skip(query.Skip)		// Pominięcie określonej liczby rekordów
				.Take(query.Take)		// Pobranie określonej liczby rekordów
				.ToListAsync();			// Konwersja na listę
		}
	}
}