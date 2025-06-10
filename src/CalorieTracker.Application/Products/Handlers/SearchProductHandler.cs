using CalorieTracker.Application.Auth.Queries;
using CalorieTracker.Application.Interfaces;
using CalorieTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.Auth.Handlers
{
	/// <summary>
	/// Handler do wyszukiwania produktów
	/// </summary>
	public class SearchProductsHandler
	{
		private readonly IAppDbContext _db;

		public SearchProductsHandler(IAppDbContext db) => _db = db;

		public async Task<List<Product>> Handle(SearchProductsQuery query)
		{
			var searchQuery = _db.Products.AsQueryable();

			// Filtrowanie po nazwie (case-insensitive, zawiera)
			if (!string.IsNullOrWhiteSpace(query.SearchTerm))
			{
				var searchTerm = query.SearchTerm.ToLower();
				searchQuery = searchQuery.Where(p =>
					p.Name.ToLower().Contains(searchTerm) ||
					(p.Brand != null && p.Brand.ToLower().Contains(searchTerm)));
			}

			// Filtrowanie po kategorii
			if (query.Category.HasValue)
			{
				searchQuery = searchQuery.Where(p => p.Category == query.Category.Value);
			}

			// Filtrowanie tylko zweryfikowane
			if (query.VerifiedOnly)
			{
				searchQuery = searchQuery.Where(p => p.IsVerified);
			}

			return await searchQuery
				.OrderBy(p => p.Name)
				.Skip(query.Skip)
				.Take(query.Take)
				.ToListAsync();
		}
	}
}
