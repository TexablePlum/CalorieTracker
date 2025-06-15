// Plik DeleteWeightMeasurementHandler.cs - implementacja handlera usuwania pomiaru wagi.
// Odpowiada za przetwarzanie komendy DeleteWeightMeasurementCommand i zarządzanie logiką biznesową związaną z usuwaniem pomiarów wagi.

using CalorieTracker.Application.Interfaces;
using CalorieTracker.Application.WeightMeasurements.Commands;
using CalorieTracker.Domain.Entities;
using CalorieTracker.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.WeightMeasurements.Handlers
{
	/// <summary>
	/// Handler odpowiedzialny za przetwarzanie komendy usuwania pomiaru masy ciała.
	/// Zarządza całą logiką związaną z walidacją, usuwaniem i aktualizacją powiązanych danych.
	/// </summary>
	public class DeleteWeightMeasurementHandler
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
		public DeleteWeightMeasurementHandler(IAppDbContext db, WeightAnalysisService weightAnalysis)
		{
			_db = db;
			_weightAnalysis = weightAnalysis;
		}

		/// <summary>
		/// Przetwarza komendę usuwania pomiaru wagi.
		/// </summary>
		/// <param name="command">Komenda <see cref="DeleteWeightMeasurementCommand"/> zawierająca identyfikator pomiaru i użytkownika.</param>
		/// <returns>Wartość boolowska wskazująca, czy operacja usunięcia zakończyła się sukcesem.</returns>
		public async Task<bool> Handle(DeleteWeightMeasurementCommand command)
		{
			var measurement = await _db.WeightMeasurements
				.FirstOrDefaultAsync(w => w.Id == command.Id && w.UserId == command.UserId);

			if (measurement is null) return false;

			var deletedDate = measurement.MeasurementDate;
			var userId = command.UserId;

			// Pobiera profil użytkownika
			var userProfile = await _db.UserProfiles
				.FirstOrDefaultAsync(p => p.UserId == userId);

			if (userProfile is null) return false;

			// Usuwa pomiar
			_db.WeightMeasurements.Remove(measurement);

			// Zapisuje usunięcie
			await _db.SaveChangesAsync();

			// Przelicza wszytskie późniejsze niż usunięty
			await RecalculateFutureMeasurements(userId, deletedDate, userProfile);

			// Jeśli usuwa najnowszy pomiar, aktualizuje profil użytkownika
			var latestMeasurement = await _db.WeightMeasurements
				.Where(w => w.UserId == userId)
				.OrderByDescending(w => w.MeasurementDate)
				.FirstOrDefaultAsync();

			if (latestMeasurement != null)
			{
				userProfile.WeightKg = latestMeasurement.WeightKg;
			}
			else
			{
				// userProfile.WeightKg = null; // Opcjonalnie
			}

			await _db.SaveChangesAsync();
			return true;
		}

		/// <summary>
		/// Przelicza wszystkie pomiary wykonane po dacie usuniętego pomiaru.
		/// Aktualizuje kalkulowane pola (BMI, zmiana wagi) dla pomiarów wykonanych po usuniętym pomiarze.
		/// </summary>
		/// <param name="userId">Identyfikator użytkownika.</param>
		/// <param name="deletedDate">Data usuniętego pomiaru.</param>
		/// <param name="userProfile">Profil użytkownika zawierający dane do obliczeń.</param>
		private async Task RecalculateFutureMeasurements(string userId, DateOnly deletedDate, UserProfile userProfile)
		{
			// Pobiera wszystkie pomiary użytkownika w kolejności chronologicznej
			var allMeasurements = await _db.WeightMeasurements
				.Where(w => w.UserId == userId)
				.OrderBy(w => w.MeasurementDate)
				.ThenBy(w => w.CreatedAt)
				.ToListAsync();

			// Pomiary do przeliczenia (późniejsze niż usunięty)
			var measurementsToRecalculate = allMeasurements
				.Where(m => m.MeasurementDate > deletedDate)
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

			// Zapisuje wszystkie zmiany
			await _db.SaveChangesAsync();
		}
	}
}