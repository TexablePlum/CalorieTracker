// Plik CreateRecipeHandler.cs - implementacja handlera polecenia tworzenia przepisu.
// Odpowiada za przetwarzanie komendy CreateRecipeCommand, walidację i zapis nowego przepisu w bazie danych.
// Specjalnie nie urzyto FluentValidation aby pokazać różne podejścia i możliwość walidacji danych bez tej biblioteki

using CalorieTracker.Application.Interfaces;
using CalorieTracker.Application.Recipes.Commands;
using CalorieTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.Recipes.Handlers
{
	/// <summary>
	/// Handler odpowiedzialny za przetwarzanie komendy tworzenia nowego przepisu.
	/// Zawiera logikę walidacji składników oraz tworzenia i zapisu przepisu w bazie danych.
	/// </summary>
	public class CreateRecipeHandler
	{
		/// <summary>
		/// Prywatne pole tylko do odczytu, przechowujące kontekst bazy danych.
		/// Umożliwia dostęp do operacji na bazie danych.
		/// </summary>
		private readonly IAppDbContext _db;

		/// <summary>
		/// Inicjalizuje nową instancję klasy <see cref="CreateRecipeHandler"/>.
		/// </summary>
		/// <param name="db">Kontekst bazy danych <see cref="IAppDbContext"/>.</param>
		public CreateRecipeHandler(IAppDbContext db) => _db = db;

		/// <summary>
		/// Przetwarza komendę tworzenia nowego przepisu.
		/// Wykonuje walidację składników, tworzy przepis i zapisuje go w bazie danych.
		/// </summary>
		/// <param name="command">Komenda <see cref="CreateRecipeCommand"/> zawierająca dane nowego przepisu.</param>
		/// <returns>Identyfikator GUID nowo utworzonego przepisu.</returns>
		/// <exception cref="ArgumentException">Wyjątek wyrzucany gdy nie znaleziono produktów będących składnikami przepisu.</exception>
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