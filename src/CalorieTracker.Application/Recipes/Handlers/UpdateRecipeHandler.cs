// Plik UpdateRecipeHandler.cs - implementacja handlera aktualizacji przepisu.
// Odpowiada za proces aktualizacji istniejącego przepisu wraz z walidacją uprawnień i składników.

using CalorieTracker.Application.Interfaces;
using CalorieTracker.Application.Recipes.Commands;
using CalorieTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.Recipes.Handlers
{
	/// <summary>
	/// Handler odpowiedzialny za przetwarzanie komendy aktualizacji przepisu.
	/// Zawiera kompleksową logikę walidacji uprawnień, składników oraz właściwości przepisu.
	/// </summary>
	public class UpdateRecipeHandler
	{
		/// <summary>
		/// Kontekst bazy danych aplikacji.
		/// Umożliwia dostęp do operacji na przepisach i powiązanych encjach.
		/// </summary>
		private readonly IAppDbContext _db;

		/// <summary>
		/// Inicjalizuje nową instancję handlera aktualizacji przepisu.
		/// </summary>
		/// <param name="db">Kontekst bazy danych implementujący IAppDbContext</param>
		public UpdateRecipeHandler(IAppDbContext db) => _db = db;

		/// <summary>
		/// Główna metoda handlera przetwarzająca komendę aktualizacji.
		/// </summary>
		/// <param name="command">Komenda zawierająca dane do aktualizacji i identyfikatory</param>
		/// <returns>
		/// True - jeśli przepis został pomyślnie zaktualizowany.
		/// False - jeśli przepis nie istnieje lub użytkownik nie ma uprawnień.
		/// </returns>
		/// <exception cref="ArgumentException">Wyjątek wyrzucany gdy nie znaleziono produktów będących składnikami</exception>
		public async Task<bool> Handle(UpdateRecipeCommand command)
		{
			// Pobranie przepisu z obecnymi składnikami
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

			// Aktualizacja podstawowych właściwości przepisu
			recipe.Name = command.Name;
			recipe.Instructions = command.Instructions;
			recipe.ServingsCount = command.ServingsCount;
			recipe.TotalWeightGrams = command.TotalWeightGrams;
			recipe.PreparationTimeMinutes = command.PreparationTimeMinutes;
			recipe.UpdatedAt = DateTime.UtcNow;

			// Usuwanie starych składników
			_db.RecipeIngredients.RemoveRange(recipe.Ingredients);

			// Zapis zmian przed dodaniem nowych składników
			await _db.SaveChangesAsync();

			// Dodawanie nowych składników
			var newIngredients = command.Ingredients.Select(ingredientCommand =>
				new RecipeIngredient
				{
					Id = Guid.NewGuid(),
					RecipeId = recipe.Id,
					ProductId = ingredientCommand.ProductId,
					Quantity = ingredientCommand.Quantity
				}).ToList();

			_db.RecipeIngredients.AddRange(newIngredients);
			await _db.SaveChangesAsync();

			return true;
		}
	}
}