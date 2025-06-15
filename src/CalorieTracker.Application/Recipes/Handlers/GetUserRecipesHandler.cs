// Plik GetUserRecipesHandler.cs - implementacja handlera zapytania o przepisy użytkownika.
// Odpowiada za pobieranie paginowanej listy przepisów stworzonych przez konkretnego użytkownika.

using CalorieTracker.Application.Interfaces;
using CalorieTracker.Application.Recipes.Queries;
using CalorieTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.Recipes.Handlers
{
	/// <summary>
	/// Handler odpowiedzialny za przetwarzanie zapytania o przepisy konkretnego użytkownika.
	/// Zapewnia pobieranie przepisów z pełnymi zależnościami z uwzględnieniem paginacji.
	/// </summary>
	public class GetUserRecipesHandler
	{
		/// <summary>
		/// Kontekst bazy danych aplikacji.
		/// Umożliwia dostęp do danych przepisów i powiązanych encji.
		/// </summary>
		private readonly IAppDbContext _db;

		/// <summary>
		/// Inicjalizuje nową instancję handlera pobierania przepisów użytkownika.
		/// </summary>
		/// <param name="db">Kontekst bazy danych implementujący IAppDbContext</param>
		public GetUserRecipesHandler(IAppDbContext db) => _db = db;

		/// <summary>
		/// Główna metoda handlera przetwarzająca zapytanie o przepisy użytkownika.
		/// </summary>
		/// <param name="query">Zapytanie zawierające identyfikator użytkownika oraz parametry paginacji</param>
		/// <returns>
		/// Lista przepisów należących do określonego użytkownika,
		/// z dołączonymi danymi autora i składnikami,
		/// posortowana malejąco według daty utworzenia.
		/// </returns>
		public async Task<List<Recipe>> Handle(GetUserRecipesQuery query)
		{
			return await _db.Recipes
				.Include(r => r.CreatedByUser)
				.Include(r => r.Ingredients)
					.ThenInclude(i => i.Product)
				.Where(r => r.CreatedByUserId == query.UserId)
				.OrderByDescending(r => r.CreatedAt)
				.Skip(query.Skip)
				.Take(query.Take)
				.ToListAsync();
		}
	}
}