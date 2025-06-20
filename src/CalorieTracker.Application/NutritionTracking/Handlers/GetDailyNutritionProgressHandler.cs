// Plik GetDailyNutritionProgressHandler.cs - handler zapytania o dzienny postęp żywieniowy.

using CalorieTracker.Application.Interfaces;
using CalorieTracker.Application.NutritionTracking.Queries;
using CalorieTracker.Domain.Services;
using CalorieTracker.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.NutritionTracking.Handlers
{
	/// <summary>
	/// Handler odpowiedzialny za przetwarzanie zapytania o dzienny postęp żywieniowy.
	/// Łączy cele użytkownika z rzeczywistym spożyciem i tworzy kompletny obraz dnia.
	/// </summary>
	public class GetDailyNutritionProgressHandler
	{
		/// <summary>
		/// Kontekst bazy danych aplikacji.
		/// </summary>
		private readonly IAppDbContext _db;

		/// <summary>
		/// Serwis domenowy do śledzenia żywienia i kalkulacji postępu.
		/// </summary>
		private readonly NutritionTrackingService _nutritionTrackingService;

		/// <summary>
		/// Inicjalizuje nową instancję handlera postępu żywieniowego.
		/// </summary>
		public GetDailyNutritionProgressHandler(
			IAppDbContext db,
			NutritionTrackingService nutritionTrackingService)
		{
			_db = db;
			_nutritionTrackingService = nutritionTrackingService;
		}

		/// <summary>
		/// Przetwarza zapytanie o dzienny postęp żywieniowy użytkownika.
		/// </summary>
		/// <param name="query">Zapytanie z ID użytkownika i datą.</param>
		/// <returns>Kompletny postęp żywieniowy lub null jeśli użytkownik nie istnieje.</returns>
		public async Task<DailyNutritionProgress?> Handle(GetDailyNutritionProgressQuery query)
		{
			var targetDate = query.EffectiveDate;

			// Pobiera profil użytkownika
			var userProfile = await _db.UserProfiles
				.FirstOrDefaultAsync(up => up.UserId == query.UserId);

			if (userProfile == null)
				return null;

			// Pobiera wszystkie posiłki z danego dnia
			var mealLogEntries = await _db.MealLogEntries
				.Include(m => m.Product)
				.Include(m => m.Recipe)
				.Where(m => m.UserId == query.UserId &&
						   DateOnly.FromDateTime(m.ConsumedAt) == targetDate)
				.OrderBy(m => m.ConsumedAt)
				.ToListAsync();

			// Pobiera dzienne spożycie wody
			var waterIntakeLiters = await GetDailyWaterIntakeInLiters(query.UserId, targetDate);

			// Tworzy kompletny postęp za pomocą serwisu domenowego
			var dailyProgress = _nutritionTrackingService.CreateDailyProgress(
				targetDate,
				userProfile,
				mealLogEntries,
				waterIntakeLiters);

			return dailyProgress;
		}

		/// <summary>
		/// Pobiera łączną ilość wypitej wody w litrach dla określonego dnia.
		/// </summary>
		/// <param name="userId">Identyfikator użytkownika.</param>
		/// <param name="date">Data do sprawdzenia.</param>
		/// <returns>Łączna ilość wody w litrach.</returns>
		private async Task<float> GetDailyWaterIntakeInLiters(string userId, DateOnly date)
		{
			var totalMilliliters = await _db.WaterIntakeLogEntries
				.Where(w => w.UserId == userId &&
						   DateOnly.FromDateTime(w.ConsumedAt) == date)
				.SumAsync(w => w.AmountMilliliters);

			return (float)(Convert.ToDecimal(totalMilliliters) / 1000m);
		}
	}
}