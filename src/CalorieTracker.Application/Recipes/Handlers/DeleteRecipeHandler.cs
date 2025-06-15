// Plik DeleteRecipeHandler.cs - implementacja handlera komendy usuwania przepisów.
// Odpowiada za proces usuwania przepisów z systemu, w tym walidację uprawnień użytkownika.

using CalorieTracker.Application.Interfaces;
using CalorieTracker.Application.Recipes.Commands;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.Recipes.Handlers
{
	/// <summary>
	/// Handler odpowiedzialny za przetwarzanie operacji usuwania przepisów.
	/// Wykonuje walidację istnienia przepisu oraz uprawnień użytkownika przed usunięciem.
	/// </summary>
	public class DeleteRecipeHandler
	{
		/// <summary>
		/// Kontekst bazy danych aplikacji.
		/// Pozwala na dostęp do operacji CRUD na przepisach.
		/// </summary>
		private readonly IAppDbContext _db;

		/// <summary>
		/// Inicjalizuje nową instancję handlera usuwania przepisów.
		/// </summary>
		/// <param name="db">Kontekst bazy danych implementujący IAppDbContext</param>
		public DeleteRecipeHandler(IAppDbContext db) => _db = db;

		/// <summary>
		/// Główna metoda handlera przetwarzająca komendę usunięcia przepisu.
		/// </summary>
		/// <param name="command">Komenda zawierająca Id przepisu do usunięcia i Id użytkownika</param>
		/// <returns>
		/// True - jeśli przepis został pomyślnie usunięty.
		/// False - jeśli przepis nie istnieje lub użytkownik nie ma uprawnień.
		/// </returns>
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