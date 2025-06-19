// Plik LogMealHandler.cs - handler komendy dodawania posiłku.
// Odpowiada za przetwarzanie LogMealCommand i zarządzanie logiką biznesową tworzenia wpisów posiłków.

using CalorieTracker.Application.Interfaces;
using CalorieTracker.Application.NutritionTracking.Commands;
using CalorieTracker.Domain.Entities;
using CalorieTracker.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.NutritionTracking.Handlers
{
	/// <summary>
	/// Handler odpowiedzialny za przetwarzanie komendy dodawania nowego posiłku do dziennika.
	/// Zarządza walidacją, kalkulacją wartości odżywczych i zapisem do bazy danych.
	/// </summary>
	public class LogMealHandler
	{
		/// <summary>
		/// Kontekst bazy danych aplikacji.
		/// </summary>
		private readonly IAppDbContext _db;

		/// <summary>
		/// Serwis domenowy do śledzenia żywienia i kalkulacji wartości odżywczych.
		/// </summary>
		private readonly NutritionTrackingService _nutritionTrackingService;

		/// <summary>
		/// Inicjalizuje nową instancję handlera dodawania posiłku.
		/// </summary>
		/// <param name="db">Kontekst bazy danych.</param>
		/// <param name="nutritionTrackingService">Serwis śledzenia żywienia.</param>
		public LogMealHandler(IAppDbContext db, NutritionTrackingService nutritionTrackingService)
		{
			_db = db;
			_nutritionTrackingService = nutritionTrackingService;
		}

		/// <summary>
		/// Przetwarza komendę dodawania nowego posiłku do dziennika żywieniowego.
		/// Waliduje dane, pobiera produkt/przepis, kalkuluje wartości odżywcze i zapisuje wpis.
		/// </summary>
		/// <param name="command">Komenda zawierająca dane nowego posiłku.</param>
		/// <returns>Unikalny identyfikator utworzonego wpisu posiłku.</returns>
		/// <exception cref="ArgumentException">Gdy dane komendy są nieprawidłowe.</exception>
		/// <exception cref="InvalidOperationException">Gdy produkt lub przepis nie istnieje.</exception>
		public async Task<Guid> Handle(LogMealCommand command)
		{
			// Walidacja podstawowych danych komendy
			ValidateCommand(command);

			// Tworzenie nowego wpisu posiłku
			var mealLogEntry = new MealLogEntry
			{
				UserId = command.UserId,
				ProductId = command.ProductId,
				RecipeId = command.RecipeId,
				Quantity = command.Quantity,
				MealType = command.MealType,
				ConsumedAt = command.ConsumedAt ?? DateTime.UtcNow,
				Notes = command.Notes
			};

			// Pobieranie i ustawianie relacjji (Product lub Recipe)
			await LoadMealLogEntryRelations(mealLogEntry);

			// Walidacja za pomocą serwisu domenowego
			if (!_nutritionTrackingService.ValidateMealLogEntry(mealLogEntry))
				throw new ArgumentException("Dane wpisu posiłku są nieprawidłowe");

			// Kalkulacja wartości odżywczych
			_nutritionTrackingService.CalculateNutritionForMealEntry(mealLogEntry);

			// Zapis do bazy danych
			_db.MealLogEntries.Add(mealLogEntry);
			await _db.SaveChangesAsync();

			return mealLogEntry.Id;
		}

		/// <summary>
		/// Waliduje podstawowe dane komendy.
		/// </summary>
		private static void ValidateCommand(LogMealCommand command)
		{
			if (string.IsNullOrEmpty(command.UserId))
				throw new ArgumentException("UserId jest wymagane");

			if (command.Quantity <= 0 || command.Quantity > 10000)
				throw new ArgumentException("Quantity musi być w przedziale 0.1-10000");

			var hasProduct = command.ProductId.HasValue;
			var hasRecipe = command.RecipeId.HasValue;

			if (hasProduct == hasRecipe)
				throw new ArgumentException("Należy podać ProductId XOR RecipeId (dokładnie jedno)");
		}

		/// <summary>
		/// Ładuje relacje (Product lub Recipe) dla wpisu posiłku z bazy danych.
		/// </summary>
		private async Task LoadMealLogEntryRelations(MealLogEntry mealLogEntry)
		{
			if (mealLogEntry.ProductId.HasValue)
			{
				mealLogEntry.Product = await _db.Products
					.FirstOrDefaultAsync(p => p.Id == mealLogEntry.ProductId.Value);

				if (mealLogEntry.Product == null)
					throw new InvalidOperationException($"Product o ID {mealLogEntry.ProductId} nie istnieje");
			}
			else if (mealLogEntry.RecipeId.HasValue)
			{
				mealLogEntry.Recipe = await _db.Recipes
					.Include(r => r.Ingredients)
					.ThenInclude(i => i.Product)
					.FirstOrDefaultAsync(r => r.Id == mealLogEntry.RecipeId.Value);

				if (mealLogEntry.Recipe == null)
					throw new InvalidOperationException($"Recipe o ID {mealLogEntry.RecipeId} nie istnieje");
			}
		}
	}
}