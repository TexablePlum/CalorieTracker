using CalorieTracker.Application.Interfaces;
using CalorieTracker.Application.Recipes.Queries;
using CalorieTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.Recipes.Handlers
{
	/// <summary>
	/// Handler do pobierania wszystkich przepisów (globalna lista)
	/// </summary>
	public class GetAllRecipesHandler
	{
		private readonly IAppDbContext _db;

		public GetAllRecipesHandler(IAppDbContext db) => _db = db;

		public async Task<List<Recipe>> Handle(GetAllRecipesQuery query)
		{
			return await _db.Recipes
				.Include(r => r.CreatedByUser)
				.OrderByDescending(r => r.CreatedAt)
				.Skip(query.Skip)
				.Take(query.Take)
				.ToListAsync();
		}
	}
}