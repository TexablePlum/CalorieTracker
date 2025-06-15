// Plik GetProductByBarcodeHandler.cs - implementacja handlera wyszukiwania produktu po kodzie kreskowym.
// Odpowiada za pobieranie danych produktu na podstawie jego kodu kreskowego.

using CalorieTracker.Application.Auth.Queries;
using CalorieTracker.Application.Interfaces;
using CalorieTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.Auth.Handlers
{
	/// <summary>
	/// Handler odpowiedzialny za przetwarzanie zapytania o produkt na podstawie kodu kreskowego.
	/// Wykonuje wyszukiwanie produktu w bazie danych i zwraca jego pełne dane.
	/// </summary>
	public class GetProductByBarcodeHandler
	{
		/// <summary>
		/// Kontekst bazy danych do wyszukiwania produktów.
		/// </summary>
		private readonly IAppDbContext _db;

		/// <summary>
		/// Inicjalizuje nową instancję handlera wyszukiwania produktu.
		/// </summary>
		/// <param name="db">Kontekst bazy danych <see cref="IAppDbContext"/>.</param>
		public GetProductByBarcodeHandler(IAppDbContext db) => _db = db;

		/// <summary>
		/// Główna metoda handlera wykonująca wyszukiwanie produktu.
		/// </summary>
		/// <param name="query">Zapytanie <see cref="GetProductByBarcodeQuery"/> zawierające kod kreskowy.</param>
		/// <returns>
		/// Task asynchroniczny zwracający:
		/// - Encję <see cref="Product"/> jeśli produkt zostanie znaleziony
		/// - null, jeśli produkt o podanym kodzie kreskowym nie istnieje w systemie
		/// </returns>
		public async Task<Product?> Handle(GetProductByBarcodeQuery query)
		{
			return await _db.Products
				.FirstOrDefaultAsync(p => p.Barcode == query.Barcode);
		}
	}
}