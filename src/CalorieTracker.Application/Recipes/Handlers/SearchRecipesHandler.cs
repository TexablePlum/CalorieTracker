using CalorieTracker.Application.Interfaces;
using CalorieTracker.Application.Recipes.Queries;
using CalorieTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.Recipes.Handlers
{
	/// <summary>
	/// Handler do wyszukiwania przepisów
	/// </summary>
	public class SearchRecipesHandler
	{
		private readonly IAppDbContext _db;

		public SearchRecipesHandler(IAppDbContext db) => _db = db;

		public async Task<List<Recipe>> Handle(SearchRecipesQuery query)
		{
			var searchQuery = _db.Recipes
				.Include(r => r.CreatedByUser)
				.Include(r => r.Ingredients)
					.ThenInclude(i => i.Product)
				.AsQueryable();

			// Wyszukiwanie po nazwie
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