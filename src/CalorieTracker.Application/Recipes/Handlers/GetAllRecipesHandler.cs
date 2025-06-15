// Plik GetAllRecipesHandler.cs - implementacja handlera zapytania o przepisy.
// Odpowiada za pobieranie i zwracanie listy przepisów z systemu z uwzględnieniem paginacji.

using CalorieTracker.Application.Interfaces;
using CalorieTracker.Application.Recipes.Queries;
using CalorieTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.Recipes.Handlers
{
	/// <summary>
	/// Handler odpowiedzialny za przetwarzanie zapytania o listę wszystkich przepisów.
	/// Zapewnia pobieranie danych z uwzględnieniem paginacji i pełnych zależności.
	/// </summary>
	public class GetAllRecipesHandler
	{
		/// <summary>
		/// Kontekst bazy danych aplikacji.
		/// Umożliwia dostęp do danych przepisów i powiązanych encji.
		/// </summary>
		private readonly IAppDbContext _db;

		/// <summary>
		/// Inicjalizuje nową instancję handlera pobierania przepisów.
		/// </summary>
		/// <param name="db">Kontekst bazy danych implementujący IAppDbContext</param>
		public GetAllRecipesHandler(IAppDbContext db) => _db = db;

		/// <summary>
		/// Główna metoda handlera przetwarzająca zapytanie o listę przepisów.
		/// </summary>
		/// <param name="query">Zapytanie zawierające parametry paginacji (Skip, Take)</param>
		/// <returns>
		/// Lista przepisów z dołączonymi danymi użytkownika oraz składnikami,
		/// posortowana malejąco według daty utworzenia.
		/// </returns>
		public async Task<List<Recipe>> Handle(GetAllRecipesQuery query)
		{
			return await _db.Recipes
				.Include(r => r.CreatedByUser)
				.Include(r => r.Ingredients)
					.ThenInclude(i => i.Product)
				.OrderByDescending(r => r.CreatedAt)
				.Skip(query.Skip)
				.Take(query.Take)
				.ToListAsync();
		}
	}
}