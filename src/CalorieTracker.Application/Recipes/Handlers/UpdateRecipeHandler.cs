using CalorieTracker.Application.Interfaces;
using CalorieTracker.Application.Recipes.Commands;
using CalorieTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.Recipes.Handlers
{
	/// <summary>
	/// Handler do aktualizacji przepisu
	/// </summary>
	public class UpdateRecipeHandler
	{
		private readonly IAppDbContext _db;

		public UpdateRecipeHandler(IAppDbContext db) => _db = db;

		public async Task<bool> Handle(UpdateRecipeCommand command)
		{
			var recipe = await _db.Recipes
				.Include(r => r.Ingredients)
				.FirstOrDefaultAsync(r => r.Id == command.Id);

			if (recipe is null) return false;

			// Sprawdzenie uprawnień - tylko właściciel może edytować
			if (recipe.CreatedByUserId != command.UpdatedByUserId)
				return false;

			// Walidacja - sprawdza czy wszystkie produkty istnieją
			var productIds = command.Ingredients.Select(i => i.ProductId).ToList();
			var existingProducts = await _db.Products
				.Where(p => productIds.Contains(p.Id))
				.Select(p => p.Id)
				.ToListAsync();

			var missingProducts = productIds.Except(existingProducts).ToList();
			if (missingProducts.Any())
			{
				throw new ArgumentException($"Nie znaleziono produktów o ID: {string.Join(", ", missingProducts)}");
			}

			// Aktualizacja danych przepisu
			recipe.Name = command.Name;
			recipe.Instructions = command.Instructions;
			recipe.ServingsCount = command.ServingsCount;
			recipe.TotalWeightGrams = command.TotalWeightGrams;
			recipe.PreparationTimeMinutes = command.PreparationTimeMinutes;
			recipe.UpdatedAt = DateTime.UtcNow;

			// Usunięcie starych składników
			_db.RecipeIngredients.RemoveRange(recipe.Ingredients);

			// Dodanie nowych składników
			recipe.Ingredients.Clear();
			foreach (var ingredientCommand in command.Ingredients)
			{
				var ingredient = new RecipeIngredient
				{
					RecipeId = recipe.Id,
					ProductId = ingredientCommand.ProductId,
					Quantity = ingredientCommand.Quantity
				};
				recipe.Ingredients.Add(ingredient);
			}

			await _db.SaveChangesAsync();
			return true;
		}
	}
}