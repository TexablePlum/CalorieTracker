// Plik CreateWeightMeasurementHandler.cs - implementacja handlera tworzenia pomiaru wagi.
// Odpowiada za przetwarzanie komendy CreateWeightMeasurementCommand i zarządzanie logiką biznesową związaną z tworzeniem nowych pomiarów wagi.

using CalorieTracker.Application.Interfaces;
using CalorieTracker.Application.WeightMeasurements.Commands;
using CalorieTracker.Domain.Entities;
using CalorieTracker.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.WeightMeasurements.Handlers
{
	/// <summary>
	/// Handler odpowiedzialny za przetwarzanie komendy tworzenia nowego pomiaru masy ciała.
	/// Zarządza całą logiką związaną z walidacją, tworzeniem i aktualizacją pomiarów wagi.
	/// </summary>
	public class CreateWeightMeasurementHandler
	{
		/// <summary>
		/// Obiekt kontekstu bazy danych aplikacji.
		/// Umożliwia dostęp do danych użytkowników i pomiarów.
		/// </summary>
		private readonly IAppDbContext _db;

		/// <summary>
		/// Serwis analizy danych wagowych.
		/// Odpowiada za obliczenia związane z BMI i zmianami wagi.
		/// </summary>
		private readonly WeightAnalysisService _weightAnalysis;

		/// <summary>
		/// Inicjalizuje nową instancję handlera.
		/// </summary>
		/// <param name="db">Kontekst bazy danych <see cref="IAppDbContext"/>.</param>
		/// <param name="weightAnalysis">Serwis analizy wagowej <see cref="WeightAnalysisService"/>.</param>
		public CreateWeightMeasurementHandler(IAppDbContext db, WeightAnalysisService weightAnalysis)
		{
			_db = db;
			_weightAnalysis = weightAnalysis;
		}

		/// <summary>
		/// Przetwarza komendę tworzenia nowego pomiaru wagi.
		/// </summary>
		/// <param name="command">Komenda <see cref="CreateWeightMeasurementCommand"/> zawierająca dane nowego pomiaru.</param>
		/// <returns>Identyfikator utworzonego pomiaru.</returns>
		/// <exception cref="InvalidOperationException">
		/// Wyjątek wyrzucany gdy użytkownik nie ma profilu lub gdy pomiar na podaną datę już istnieje.
		/// </exception>
		public async Task<Guid> Handle(CreateWeightMeasurementCommand command)
		{
			// Sprawdza czy użytkownik ma profil
			var userProfile = await _db.UserProfiles
				.FirstOrDefaultAsync(p => p.UserId == command.UserId);

			if (userProfile is null)
			{
				throw new InvalidOperationException("Użytkownik musi mieć uzupełniony profil aby dodawać pomiary masy ciała");
			}

			// Sprawdza czy nie ma już pomiaru na tę datę
			var existingMeasurement = await _db.WeightMeasurements
				.FirstOrDefaultAsync(w => w.UserId == command.UserId && w.MeasurementDate == command.MeasurementDate);

			if (existingMeasurement != null)
			{
				throw new InvalidOperationException("Pomiar na tę datę już istnieje");
			}

			// Pobiera poprzedni pomiar (najnowszy przed tą datą)
			var previousMeasurement = await _db.WeightMeasurements
				.Where(w => w.UserId == command.UserId && w.MeasurementDate < command.MeasurementDate)
				.OrderByDescending(w => w.MeasurementDate)
				.FirstOrDefaultAsync();

			// Tworzy nowy pomiar
			var measurement = new WeightMeasurement
			{
				UserId = command.UserId,
				MeasurementDate = command.MeasurementDate,
				WeightKg = command.WeightKg,
				CreatedAt = DateTime.UtcNow,
				UpdatedAt = DateTime.UtcNow
			};

			// Wypełnia kalkulowane pola (BMI, zmiana wagi)
			_weightAnalysis.FillCalculatedFields(measurement, userProfile, previousMeasurement);

			// Dodaje do bazy
			_db.WeightMeasurements.Add(measurement);

			// Zapisuje nowy pomiar
			await _db.SaveChangesAsync();

			// Przelicza przyszłe pomiary
			await RecalculateFutureMeasurements(command.UserId, command.MeasurementDate, userProfile);

			// Aktualizujw wagę w profilu użytkownika jeśli to najnowszy pomiar
			var latestMeasurement = await _db.WeightMeasurements
				.Where(w => w.UserId == command.UserId)
				.OrderByDescending(w => w.MeasurementDate)
				.FirstOrDefaultAsync();

			if (latestMeasurement?.Id == measurement.Id)
			{
				userProfile.WeightKg = command.WeightKg;
				await _db.SaveChangesAsync();
			}

			return measurement.Id;
		}

		/// <summary>
		/// Przelicza wszystkie przyszłe pomiary dla danego użytkownika od określonej daty.
		/// Aktualizuje kalkulowane pola (BMI, zmiana wagi) dla pomiarów wykonanych po dodanym pomiarze.
		/// </summary>
		/// <param name="userId">Identyfikator użytkownika.</param>
		/// <param name="fromDate">Data początkowa, od której przeliczać pomiary.</param>
		/// <param name="userProfile">Profil użytkownika zawierający dane do obliczeń.</param>
		private async Task RecalculateFutureMeasurements(string userId, DateOnly fromDate, UserProfile userProfile)
		{
			// Pobiera wszystkie pomiary użytkownika w kolejności chronologicznej
			var allMeasurements = await _db.WeightMeasurements
				.Where(w => w.UserId == userId)
				.OrderBy(w => w.MeasurementDate)
				.ThenBy(w => w.CreatedAt)
				.ToListAsync();

			// Znajduje pomiary do przeliczenia (późniejsze niż fromDate)
			var measurementsToRecalculate = allMeasurements
				.Where(m => m.MeasurementDate > fromDate)
				.ToList();

			// Przelicza każdy pomiar
			foreach (var measurementToRecalc in measurementsToRecalculate)
			{
				// Znajduje poprzedni pomiar (najnowszy przed tym pomiarem)
				var previousMeasurement = allMeasurements
					.Where(m => m.MeasurementDate < measurementToRecalc.MeasurementDate)
					.OrderByDescending(m => m.MeasurementDate)
					.ThenByDescending(m => m.CreatedAt)
					.FirstOrDefault();

				// Przelicza kalkulowane pola
				_weightAnalysis.FillCalculatedFields(measurementToRecalc, userProfile, previousMeasurement);
				measurementToRecalc.UpdatedAt = DateTime.UtcNow;
			}

			// Zapisuje zmiany
			await _db.SaveChangesAsync();
		}
	}
}