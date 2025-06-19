// Plik DailyNutritionProgressDto.cs - główny DTO postępu żywieniowego.
// Model transferu danych dla endpointu GET /daily-progress.

namespace CalorieTracker.Api.Models.NutritionTracking
{
	/// <summary>
	/// Model transferu danych reprezentujący kompletny dzienny postęp żywieniowy.
	/// Zawiera cele, spożycie, pozostałe potrzeby, procenty postępu i listę posiłków.
	/// </summary>
	public class DailyNutritionProgressDto
	{
		/// <summary>
		/// Data, której dotyczy ten postęp żywieniowy.
		/// </summary>
		public string Date { get; set; } = null!;

		/// <summary>
		/// Podstawowe wskaźniki metaboliczne.
		/// </summary>
		public MetabolicDataDto MetabolicData { get; set; } = null!;

		/// <summary>
		/// Dzienne cele żywieniowe użytkownika.
		/// </summary>
		public NutritionGoalsDto Goals { get; set; } = null!;

		/// <summary>
		/// Rzeczywiste spożyte wartości odżywcze.
		/// </summary>
		public ConsumedNutritionDto Consumed { get; set; } = null!;

		/// <summary>
		/// Pozostałe do spożycia wartości dla osiągnięcia celów.
		/// </summary>
		public RemainingNutritionDto Remaining { get; set; } = null!;

		/// <summary>
		/// Procentowy postęp względem celów dziennych.
		/// </summary>
		public ProgressPercentagesDto ProgressPercentages { get; set; } = null!;

		/// <summary>
		/// Lista posiłków zalogowanych w tym dniu.
		/// </summary>
		public List<MealLogSummaryDto> MealsToday { get; set; } = new();
	}
}