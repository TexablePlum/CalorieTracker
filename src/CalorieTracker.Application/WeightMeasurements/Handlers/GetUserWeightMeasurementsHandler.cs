// Plik GetUserWeightMeasurementsHandler.cs - implementacja handlera pobierania pomiarów wagi użytkownika.
// Logika biznesowa związana z pobieraniem paginowanej listy pomiarów masy ciała.

using CalorieTracker.Application.Interfaces;
using CalorieTracker.Application.WeightMeasurements.Queries;
using CalorieTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.WeightMeasurements.Handlers
{
	/// <summary>
	/// Handler odpowiedzialny za przetwarzanie zapytania o listę pomiarów masy ciała użytkownika.
	/// </summary>
	public class GetUserWeightMeasurementsHandler
	{
		/// <summary>
		/// Kontekst bazy danych aplikacji.
		/// </summary>
		private readonly IAppDbContext _db;

		/// <summary>
		/// Inicjalizacja handlera z wymaganymi zależnościami.
		/// </summary>
		/// <param name="db">Kontekst bazy danych <see cref="IAppDbContext"/>.</param>
		public GetUserWeightMeasurementsHandler(IAppDbContext db) => _db = db;

		/// <summary>
		/// Przetwarzanie zapytania o listę pomiarów wagi użytkownika z uwzględnieniem paginacji.
		/// </summary>
		/// <param name="query">Zapytanie <see cref="GetUserWeightMeasurementsQuery"/> zawierające parametry wyszukiwania.</param>
		/// <returns>Lista pomiarów wagi użytkownika w postaci <see cref="List{WeightMeasurement}"/>.</returns>
		public async Task<List<WeightMeasurement>> Handle(GetUserWeightMeasurementsQuery query)
		{
			return await _db.WeightMeasurements
				.Where(w => w.UserId == query.UserId)
				.OrderByDescending(w => w.MeasurementDate)
				.Skip(query.Skip)
				.Take(query.Take)
				.ToListAsync();
		}
	}
}