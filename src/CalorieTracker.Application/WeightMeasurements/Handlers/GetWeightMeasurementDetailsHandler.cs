// Plik GetWeightMeasurementDetailsHandler.cs - implementacja handlera zapytania o szczegóły pomiaru wagi.
// Odpowiada za pobieranie szczegółowych informacji o konkretnym pomiarze wagi dla danego użytkownika.

using CalorieTracker.Application.Interfaces;
using CalorieTracker.Application.WeightMeasurements.Queries;
using CalorieTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.WeightMeasurements.Handlers
{
	/// <summary>
	/// Handler zapytania o szczegóły pomiaru wagi.
	/// Implementuje logikę pobierania pojedynczego pomiaru wagi na podstawie identyfikatora pomiaru i użytkownika.
	/// </summary>
	public class GetWeightMeasurementDetailsHandler
	{
		/// <summary>
		/// Prywatne pole tylko do odczytu, przechowujące kontekst bazy danych.
		/// Umożliwia dostęp do danych pomiarów wagi.
		/// </summary>
		private readonly IAppDbContext _db;

		/// <summary>
		/// Inicjalizuje nową instancję klasy <see cref="GetWeightMeasurementDetailsHandler"/>.
		/// </summary>
		/// <param name="db">Kontekst bazy danych typu <see cref="IAppDbContext"/>.</param>
		public GetWeightMeasurementDetailsHandler(IAppDbContext db) => _db = db;

		/// <summary>
		/// Obsługuje zapytanie o szczegóły pomiaru wagi.
		/// Pobiera z bazy danych pomiar wagi o podanym identyfikatorze, sprawdzając jednocześnie,
		/// czy należy do wskazanego użytkownika.
		/// </summary>
		/// <param name="query">Obiekt zapytania <see cref="GetWeightMeasurementDetailsQuery"/> zawierający identyfikator pomiaru i użytkownika.</param>
		/// <returns>Obiekt <see cref="WeightMeasurement"/> reprezentujący pomiar wagi lub null, jeśli nie znaleziono pasującego rekordu.</returns>
		public async Task<WeightMeasurement?> Handle(GetWeightMeasurementDetailsQuery query)
		{
			return await _db.WeightMeasurements
				.FirstOrDefaultAsync(w => w.Id == query.Id && w.UserId == query.UserId);
		}
	}
}