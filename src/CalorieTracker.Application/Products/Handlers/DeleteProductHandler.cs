// Plik DeleteProductHandler.cs - implementacja handlera usuwania produktu.
// Odpowiada za proces usuwania produktu z systemu z uwzględnieniem kontroli uprawnień.

using CalorieTracker.Application.Auth.Commands;
using CalorieTracker.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.Auth.Handlers
{
	/// <summary>
	/// Handler odpowiedzialny za przetwarzanie komendy usuwania produktu.
	/// Wykonuje walidację uprawnień i usuwa produkt z bazy danych.
	/// </summary>
	public class DeleteProductHandler
	{
		/// <summary>
		/// Kontekst bazy danych do zarządzania produktami.
		/// </summary>
		private readonly IAppDbContext _db;

		/// <summary>
		/// Inicjalizuje nową instancję handlera usuwania produktu.
		/// </summary>
		/// <param name="db">Kontekst bazy danych <see cref="IAppDbContext"/>.</param>
		public DeleteProductHandler(IAppDbContext db) => _db = db;

		/// <summary>
		/// Główna metoda handlera wykonująca proces usuwania produktu.
		/// </summary>
		/// <param name="command">Komenda <see cref="DeleteProductCommand"/> zawierająca identyfikator produktu i użytkownika.</param>
		/// <returns>
		/// Task asynchroniczny zwracający:
		/// - true, jeśli produkt został pomyślnie usunięty
		/// - false, jeśli produkt nie istnieje lub użytkownik nie ma uprawnień
		/// </returns>
		public async Task<bool> Handle(DeleteProductCommand command)
		{
			// Wyszukuje produkt do usunięcia
			var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == command.Id);
			if (product is null) return false;

			// Sprawdza uprawnienia użytkownika
			if (product.CreatedByUserId != command.DeletedByUserId && !await IsAdmin(command.DeletedByUserId))
				return false;

			// Usuwa produkt z bazy danych
			_db.Products.Remove(product);
			await _db.SaveChangesAsync();

			return true;
		}

		/// <summary>
		/// Metoda pomocnicza sprawdzająca, czy użytkownik ma uprawnienia administratora.
		/// </summary>
		/// <param name="userId">Identyfikator użytkownika do weryfikacji.</param>
		/// <returns>
		/// Task asynchroniczny zwracający:
		/// - true, jeśli użytkownik jest administratorem
		/// - false w przeciwnym przypadku (obecnie zawsze false - funkcjonalność do implementacji)
		/// </returns>
		/// <remarks>
		/// TODO: W przyszłości należy zaimplementować prawidłową weryfikację ról użytkowników.
		/// Obecnie zawsze zwraca false, co oznacza, że tylko twórca produktu może go usunąć.
		/// </remarks>
		private Task<bool> IsAdmin(string userId)
		{
			return Task.FromResult(false); // TODO: Implementacja w przyszłości
		}
	}
}