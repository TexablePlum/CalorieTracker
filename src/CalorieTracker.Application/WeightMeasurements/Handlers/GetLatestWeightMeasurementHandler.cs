// Plik GetLatestWeightMeasurementHandler.cs - implementacja handlera pobierania najnowszego pomiaru wagi.
// Logika biznesowa związana z wyszukiwaniem ostatniego pomiaru masy ciała dla określonego użytkownika.

using CalorieTracker.Application.Interfaces;
using CalorieTracker.Application.WeightMeasurements.Queries;
using CalorieTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.WeightMeasurements.Handlers
{
	/// <summary>
	/// Handler odpowiedzialny za przetwarzanie zapytania o najnowszy pomiar masy ciała.
	/// </summary>
	public class GetLatestWeightMeasurementHandler
	{
		/// <summary>
		/// Kontekst bazy danych aplikacji.
		/// </summary>
		private readonly IAppDbContext _db;

		/// <summary>
		/// Inicjalizacja handlera z wymaganymi zależnościami.
		/// </summary>
		/// <param name="db">Kontekst bazy danych <see cref="IAppDbContext"/>.</param>
		public GetLatestWeightMeasurementHandler(IAppDbContext db) => _db = db;

		/// <summary>
		/// Przetwarzanie zapytania o najnowszy pomiar wagi użytkownika.
		/// </summary>
		/// <param name="query">Zapytanie <see cref="GetLatestWeightMeasurementQuery"/> zawierające identyfikator użytkownika.</param>
		/// <returns>Najnowszy pomiar wagi użytkownika lub null, jeśli brak pomiarów.</returns>
		public async Task<WeightMeasurement?> Handle(GetLatestWeightMeasurementQuery query)
		{
			return await _db.WeightMeasurements
				.Where(w => w.UserId == query.UserId)
				.OrderByDescending(w => w.MeasurementDate)
				.FirstOrDefaultAsync();
		}
	}
}