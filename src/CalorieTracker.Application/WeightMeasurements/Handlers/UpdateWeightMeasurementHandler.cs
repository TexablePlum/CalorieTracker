// Plik UpdateWeightMeasurementHandler.cs - implementacja handlera aktualizacji pomiaru wagi.
// Odpowiada za przetwarzanie komendy UpdateWeightMeasurementCommand i zarządzanie logiką biznesową związaną z modyfikacją istniejących pomiarów wagi.

using CalorieTracker.Application.Interfaces;
using CalorieTracker.Application.WeightMeasurements.Commands;
using CalorieTracker.Application.WeightMeasurements.Services;
using CalorieTracker.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.WeightMeasurements.Handlers
{
	/// <summary>
	/// Handler odpowiedzialny za przetwarzanie komendy aktualizacji istniejącego pomiaru masy ciała.
	/// Zarządza całą logiką związaną z walidacją uprawnień, modyfikacją danych oraz przeliczaniem powiązanych pomiarów.
	/// Automatycznie aktualizuje kalkulowane pola w całej sekwencji pomiarów, które mogły zostać dotknięte zmianą.
	/// </summary>
	public class UpdateWeightMeasurementHandler
	{
		/// <summary>
		/// Kontekst bazy danych do zarządzania pomiarami wagi i profilami użytkowników.
		/// </summary>
		private readonly IAppDbContext _db;

		/// <summary>
		/// Serwis przeliczania pomiarów odpowiedzialny za aktualizację kalkulowanych pól w sekwencji pomiarów.
		/// </summary>
		private readonly WeightMeasurementRecalculationService _recalculationService;

		/// <summary>
		/// Inicjalizuje nową instancję handlera aktualizacji pomiaru wagi.
		/// </summary>
		/// <param name="db">Kontekst bazy danych <see cref="IAppDbContext"/>.</param>
		/// <param name="recalculationService">Serwis przeliczania pomiarów <see cref="WeightMeasurementRecalculationService"/>.</param>
		public UpdateWeightMeasurementHandler(
			IAppDbContext db,
			WeightMeasurementRecalculationService recalculationService)
		{
			_db = db;
			_recalculationService = recalculationService;
		}

		/// <summary>
		/// Przetwarza komendę aktualizacji istniejącego pomiaru masy ciała.
		/// Wykonuje walidację uprawnień dostępu, aktualizuje dane pomiaru, przelicza wszystkie pomiary 
		/// w zakresie dat, które mogły zostać dotknięte zmianą oraz aktualizuje profil użytkownika 
		/// jeśli zmodyfikowany pomiar jest najnowszy.
		/// </summary>
		/// <param name="command">Komenda <see cref="UpdateWeightMeasurementCommand"/> zawierająca nowe dane pomiaru oraz identyfikatory.</param>
		/// <returns>
		/// Task asynchroniczny zwracający wartość boolowską:
		/// - true - gdy aktualizacja przebiegła pomyślnie
		/// - false - gdy pomiar nie został znaleziony, nie należy do użytkownika lub użytkownik nie ma profilu
		/// </returns>
		/// <remarks>
		/// Metoda automatycznie przelicza wszystkie pomiary od najwcześniejszej z dat (starej i nowej),
		/// aby zapewnić spójność kalkulowanych pól BMI i zmian wagi w całej sekwencji pomiarów.
		/// </remarks>
		public async Task<bool> Handle(UpdateWeightMeasurementCommand command)
		{
			var measurement = await _db.WeightMeasurements
				.FirstOrDefaultAsync(w => w.Id == command.Id && w.UserId == command.UserId);
			if (measurement is null) return false;

			var userProfile = await _db.UserProfiles
				.FirstOrDefaultAsync(p => p.UserId == command.UserId);
			if (userProfile is null) return false;

			var oldDate = measurement.MeasurementDate;

			// Aktualizuje dane pomiaru
			measurement.MeasurementDate = command.MeasurementDate;
			measurement.WeightKg = command.WeightKg;
			measurement.UpdatedAt = DateTime.UtcNow;
			await _db.SaveChangesAsync();

			// Przelicza pomiary które mogły być dotknięte zmianą
			await _recalculationService.RecalculateFromEarliest(command.UserId, oldDate, command.MeasurementDate, userProfile);

			// Sprawdza czy to najnowszy pomiar
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
	}
}