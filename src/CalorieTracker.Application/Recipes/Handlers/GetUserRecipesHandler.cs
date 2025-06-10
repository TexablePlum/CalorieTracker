using CalorieTracker.Application.Interfaces;
using CalorieTracker.Application.Recipes.Queries;
using CalorieTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.Recipes.Handlers
{
	/// <summary>
	/// Handler do pobierania przepisów użytkownika
	/// </summary>
	public class GetUserRecipesHandler
	{
		private readonly IAppDbContext _db;

		public GetUserRecipesHandler(IAppDbContext db) => _db = db;

		public async Task<List<Recipe>> Handle(GetUserRecipesQuery query)
		{
			return await _db.Recipes
				.Include(r => r.CreatedByUser)
				.Where(r => r.CreatedByUserId == query.UserId)
				.OrderByDescending(r => r.CreatedAt)
				.Skip(query.Skip)
				.Take(query.Take)
				.ToListAsync();
		}
	}
}