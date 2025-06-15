// Plik SearchRecipesHandler.cs - implementacja handlera wyszukiwania przepisów.
// Odpowiada za przeszukiwanie przepisów na podstawie podanej frazy z uwzględnieniem paginacji.

using CalorieTracker.Application.Interfaces;
using CalorieTracker.Application.Recipes.Queries;
using CalorieTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.Recipes.Handlers
{
	/// <summary>
	/// Handler odpowiedzialny za przetwarzanie zapytań wyszukiwania przepisów.
	/// Wykonuje filtrowanie przepisów według nazwy z zachowaniem paginacji i ładowaniem zależności.
	/// </summary>
	public class SearchRecipesHandler
	{
		/// <summary>
		/// Kontekst bazy danych aplikacji.
		/// Umożliwia dostęp do danych przepisów i powiązanych encji.
		/// </summary>
		private readonly IAppDbContext _db;

		/// <summary>
		/// Inicjalizuje nową instancję handlera wyszukiwania przepisów.
		/// </summary>
		/// <param name="db">Kontekst bazy danych implementujący IAppDbContext</param>
		public SearchRecipesHandler(IAppDbContext db) => _db = db;

		/// <summary>
		/// Główna metoda handlera przetwarzająca zapytanie wyszukiwania.
		/// </summary>
		/// <param name="query">Zapytanie zawierające frazę wyszukiwania oraz parametry paginacji</param>
		/// <returns>
		/// Lista przepisów spełniających kryteria wyszukiwania,
		/// z dołączonymi danymi autora i składnikami,
		/// posortowana alfabetycznie według nazwy.
		/// </returns>
		public async Task<List<Recipe>> Handle(SearchRecipesQuery query)
		{
			var searchQuery = _db.Recipes
				.Include(r => r.CreatedByUser)
				.Include(r => r.Ingredients)
					.ThenInclude(i => i.Product)
				.AsQueryable();

			// Wyszukiwanie po nazwie
			if (!string.IsNullOrWhiteSpace(query.SearchTerm))
			{
				var searchTerm = query.SearchTerm.ToLower();
				searchQuery = searchQuery.Where(r =>
					r.Name.ToLower().Contains(searchTerm));
			}

			return await searchQuery
				.OrderBy(r => r.Name)
				.Skip(query.Skip)
				.Take(query.Take)
				.ToListAsync();
		}
	}
}