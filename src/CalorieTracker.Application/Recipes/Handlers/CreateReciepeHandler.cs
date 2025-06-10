using CalorieTracker.Application.Interfaces;
using CalorieTracker.Application.Recipes.Commands;
using CalorieTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.Recipes.Handlers
{
	/// <summary>
	/// Handler do tworzenia nowego przepisu
	/// </summary>
	public class CreateRecipeHandler
	{
		private readonly IAppDbContext _db;

		public CreateRecipeHandler(IAppDbContext db) => _db = db;

		public async Task<Guid> Handle(CreateRecipeCommand command)
		{
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

			// Tworzenie przepisu
			var recipe = new Recipe
			{
				Name = command.Name,
				Instructions = command.Instructions,
				ServingsCount = command.ServingsCount,
				TotalWeightGrams = command.TotalWeightGrams,
				PreparationTimeMinutes = command.PreparationTimeMinutes,
				CreatedByUserId = command.CreatedByUserId,
				CreatedAt = DateTime.UtcNow,
				UpdatedAt = DateTime.UtcNow
			};

			// Dodawanie składników
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

			_db.Recipes.Add(recipe);
			await _db.SaveChangesAsync();

			return recipe.Id;
		}
	}
}