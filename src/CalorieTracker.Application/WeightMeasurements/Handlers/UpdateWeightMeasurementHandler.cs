// Plik UpdateWeightMeasurementHandler.cs - implementacja handlera aktualizacji pomiaru wagi.
// Odpowiada za aktualizację istniejącego pomiaru wagi i przeliczanie powiązanych danych.

using CalorieTracker.Application.Interfaces;
using CalorieTracker.Application.WeightMeasurements.Commands;
using CalorieTracker.Domain.Entities;
using CalorieTracker.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.WeightMeasurements.Handlers
{
	/// <summary>
	/// Handler do aktualizacji pomiaru masy ciała.
	/// Zarządza procesem aktualizacji pomiaru wagi i automatycznie przelicza powiązane dane.
	/// </summary>
	public class UpdateWeightMeasurementHandler
	{
		/// <summary>
		/// Kontekst bazy danych umożliwiający dostęp do pomiarów wagi.
		/// </summary>
		private readonly IAppDbContext _db;

		/// <summary>
		/// Serwis do analizy i przeliczania danych wagowych.
		/// </summary>
		private readonly WeightAnalysisService _weightAnalysis;

		/// <summary>
		/// Inicjalizuje nową instancję handlera.
		/// </summary>
		/// <param name="db">Kontekst bazy danych</param>
		/// <param name="weightAnalysis">Serwis analizy wagowej</param>
		public UpdateWeightMeasurementHandler(IAppDbContext db, WeightAnalysisService weightAnalysis)
		{
			_db = db;
			_weightAnalysis = weightAnalysis;
		}

		/// <summary>
		/// Obsługuje komendę aktualizacji pomiaru wagi.
		/// Aktualizuje pomiar i przelicza powiązane dane jeśli to konieczne.
		/// </summary>
		/// <param name="command">Komenda aktualizacji z nowymi danymi pomiaru</param>
		/// <returns>True jeśli aktualizacja się powiodła, false jeśli nie znaleziono pomiaru lub profilu użytkownika</returns>
		public async Task<bool> Handle(UpdateWeightMeasurementCommand command)
		{
			var measurement = await _db.WeightMeasurements
				.FirstOrDefaultAsync(w => w.Id == command.Id && w.UserId == command.UserId);

			if (measurement is null) return false;

			// Pobiera profil użytkownika
			var userProfile = await _db.UserProfiles
				.FirstOrDefaultAsync(p => p.UserId == command.UserId);

			if (userProfile is null) return false;

			var oldDate = measurement.MeasurementDate;

			// Aktualizuje dane pomiaru
			measurement.MeasurementDate = command.MeasurementDate;
			measurement.WeightKg = command.WeightKg;
			measurement.UpdatedAt = DateTime.UtcNow;

			// Zapisuje zmiany
			await _db.SaveChangesAsync();

			// Przelicza pomiary które mogły być dotknięte zmianą
			await RecalculateAffectedMeasurements(command.UserId, oldDate, command.MeasurementDate, userProfile);

			// Sprawdza czy to najnowszy pomiar - jeśli tak, aktualizuje profil
			var latestMeasurement = await _db.WeightMeasurements
				.Where(w => w.UserId == command.UserId)
				.OrderByDescending(w => w.MeasurementDate)
				.FirstOrDefaultAsync();

			if (latestMeasurement?.Id == measurement.Id)
			{
				userProfile.WeightKg = command.WeightKg;
				await _db.SaveChangesAsync();
			}

			return true;
		}

		/// <summary>
		/// Przelicza pomiary dotknięte zmianą (od najwcześniejszej daty do końca).
		/// </summary>
		/// <param name="userId">Identyfikator użytkownika</param>
		/// <param name="oldDate">Poprzednia data pomiaru</param>
		/// <param name="newDate">Nowa data pomiaru</param>
		/// <param name="userProfile">Profil użytkownika</param>
		private async Task RecalculateAffectedMeasurements(string userId, DateOnly oldDate, DateOnly newDate, UserProfile userProfile)
		{
			// Znajduje najwcześniejszą datę
			var earliestAffectedDate = oldDate < newDate ? oldDate : newDate;

			// Pobiera wszystkie pomiary użytkownika w kolejności chronologicznej
			var allMeasurements = await _db.WeightMeasurements
				.Where(w => w.UserId == userId)
				.OrderBy(w => w.MeasurementDate)
				.ThenBy(w => w.CreatedAt)
				.ToListAsync();

			// Pomiary do przeliczenia (od najwcześniejszej daty)
			var measurementsToRecalculate = allMeasurements
				.Where(m => m.MeasurementDate >= earliestAffectedDate)
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