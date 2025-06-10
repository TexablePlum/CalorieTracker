using CalorieTracker.Application.Interfaces;
using CalorieTracker.Application.Recipes.Commands;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.Recipes.Handlers
{
	/// <summary>
	/// Handler do usuwania przepisu
	/// </summary>
	public class DeleteRecipeHandler
	{
		private readonly IAppDbContext _db;

		public DeleteRecipeHandler(IAppDbContext db) => _db = db;

		public async Task<bool> Handle(DeleteRecipeCommand command)
		{
			var recipe = await _db.Recipes.FirstOrDefaultAsync(r => r.Id == command.Id);
			if (recipe is null) return false;

			// Sprawdzenie uprawnień - tylko właściciel może usunąć
			if (recipe.CreatedByUserId != command.DeletedByUserId)
				return false;

			_db.Recipes.Remove(recipe);
			await _db.SaveChangesAsync();

			return true;
		}
	}
}
