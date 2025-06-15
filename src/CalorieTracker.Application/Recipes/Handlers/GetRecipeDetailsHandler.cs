// Plik GetRecipeDetailsHandler.cs - implementacja handlera zapytania o szczegóły przepisu.
// Odpowiada za pobieranie pełnych informacji o konkretnym przepisie wraz z danymi powiązanymi.

using CalorieTracker.Application.Interfaces;
using CalorieTracker.Application.Recipes.Queries;
using CalorieTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.Recipes.Handlers
{
	/// <summary>
	/// Handler odpowiedzialny za przetwarzanie zapytania o szczegółowe dane przepisu.
	/// Zapewnia pobieranie kompleksowych informacji o przepisie wraz z jego składnikami i autorem.
	/// </summary>
	public class GetRecipeDetailsHandler
	{
		/// <summary>
		/// Kontekst bazy danych aplikacji.
		/// Umożliwia dostęp do danych przepisów i powiązanych encji.
		/// </summary>
		private readonly IAppDbContext _db;

		/// <summary>
		/// Inicjalizuje nową instancję handlera pobierania szczegółów przepisu.
		/// </summary>
		/// <param name="db">Kontekst bazy danych implementujący IAppDbContext</param>
		public GetRecipeDetailsHandler(IAppDbContext db) => _db = db;

		/// <summary>
		/// Główna metoda handlera przetwarzająca zapytanie o szczegóły przepisu.
		/// </summary>
		/// <param name="query">Zapytanie zawierające identyfikator przepisu</param>
		/// <returns>
		/// Obiekt Recipe z dołączonymi danymi autora i pełną listą składników z produktami
		/// lub null jeśli przepis o podanym identyfikatorze nie istnieje.
		/// </returns>
		public async Task<Recipe?> Handle(GetRecipeDetailsQuery query)
		{
			return await _db.Recipes
				.Include(r => r.CreatedByUser)
				.Include(r => r.Ingredients)
					.ThenInclude(i => i.Product)
				.FirstOrDefaultAsync(r => r.Id == query.Id);
		}
	}
}