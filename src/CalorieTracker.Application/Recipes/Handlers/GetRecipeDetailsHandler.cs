using CalorieTracker.Application.Interfaces;
using CalorieTracker.Application.Recipes.Queries;
using CalorieTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.Recipes.Handlers
{
	/// <summary>
	/// Handler do pobierania szczegółów przepisu
	/// </summary>
	public class GetRecipeDetailsHandler
	{
		private readonly IAppDbContext _db;

		public GetRecipeDetailsHandler(IAppDbContext db) => _db = db;

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