// Plik DeleteWeightMeasurementHandler.cs - implementacja handlera usuwania pomiaru wagi.
// Odpowiada za przetwarzanie komendy DeleteWeightMeasurementCommand i zarządzanie logiką biznesową związaną z usuwaniem pomiarów wagi.

using CalorieTracker.Application.Interfaces;
using CalorieTracker.Application.WeightMeasurements.Commands;
using CalorieTracker.Application.WeightMeasurements.Services;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.WeightMeasurements.Handlers
{
	/// <summary>
	/// Handler odpowiedzialny za przetwarzanie komendy usuwania pomiaru masy ciała.
	/// Zarządza całą logiką związaną z walidacją uprawnień, usuwaniem danych oraz przeliczaniem pozostałych pomiarów.
	/// Automatycznie aktualizuje kalkulowane pola w pomiarach następujących po usuniętym oraz profil użytkownika.
	/// </summary>
	public class DeleteWeightMeasurementHandler
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
		/// Inicjalizuje nową instancję handlera usuwania pomiaru wagi.
		/// </summary>
		/// <param name="db">Kontekst bazy danych <see cref="IAppDbContext"/>.</param>
		/// <param name="recalculationService">Serwis przeliczania pomiarów <see cref="WeightMeasurementRecalculationService"/>.</param>
		public DeleteWeightMeasurementHandler(
			IAppDbContext db,
			WeightMeasurementRecalculationService recalculationService)
		{
			_db = db;
			_recalculationService = recalculationService;
		}

		/// <summary>
		/// Przetwarza komendę usuwania pomiaru masy ciała.
		/// Wykonuje walidację uprawnień dostępu, usuwa pomiar z bazy danych, przelicza wszystkie pomiary 
		/// następujące po usuniętym oraz aktualizuje profil użytkownika z wagą z najnowszego pomiaru.
		/// Operacja jest nieodwracalna - pomiar zostaje permanentnie usunięty z systemu.
		/// </summary>
		/// <param name="command">Komenda <see cref="DeleteWeightMeasurementCommand"/> zawierająca identyfikator pomiaru i użytkownika.</param>
		/// <returns>
		/// Task asynchroniczny zwracający wartość boolowską:
		/// - true - gdy usunięcie przebiegło pomyślnie
		/// - false - gdy pomiar nie został znaleziony, nie należy do użytkownika lub użytkownik nie ma profilu
		/// </returns>
		/// <remarks>
		/// Po usunięciu pomiaru metoda automatycznie:
		/// 1. Przelicza wszystkie pomiary z datami późniejszymi niż usunięty pomiar
		/// 2. Aktualizuje wagę w profilu użytkownika na podstawie najnowszego pozostałego pomiaru
		/// 3. Zachowuje spójność kalkulowanych pól BMI i zmian wagi w całej sekwencji
		/// </remarks>
		public async Task<bool> Handle(DeleteWeightMeasurementCommand command)
		{
			var measurement = await _db.WeightMeasurements
				.FirstOrDefaultAsync(w => w.Id == command.Id && w.UserId == command.UserId);
			if (measurement is null) return false;

			var userProfile = await _db.UserProfiles
				.FirstOrDefaultAsync(p => p.UserId == command.UserId);
			if (userProfile is null) return false;

			var deletedDate = measurement.MeasurementDate;

			// Usuwa pomiar
			_db.WeightMeasurements.Remove(measurement);
			await _db.SaveChangesAsync();

			// Przelicza wszystkie późniejsze pomiary
			await _recalculationService.RecalculateAfterDate(command.UserId, deletedDate, userProfile);

			// Aktualizuje profil użytkownika
			var latestMeasurement = await _db.WeightMeasurements
				.Where(w => w.UserId == command.UserId)
				.OrderByDescending(w => w.MeasurementDate)
				.FirstOrDefaultAsync();

			if (latestMeasurement != null)
			{
				userProfile.WeightKg = latestMeasurement.WeightKg;
			}
			await _db.SaveChangesAsync();

			return true;
		}
	}
}