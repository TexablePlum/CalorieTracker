// Plik GetDailyNutritionRequirementsQuery.cs - zapytanie o dzienne zapotrzebowanie żywieniowe.
// Implementuje wzorzec CQRS dla odczytu kalkulacji nutricyjnych użytkownika.

namespace CalorieTracker.Application.Nutrition.Queries
{
	/// <summary>
	/// Zapytanie o dzienne zapotrzebowanie żywieniowe użytkownika.
	/// Część wzorca CQRS do pobierania wyliczonych potrzeb kalorycznych i makroskładników.
	/// </summary>
	public class GetDailyNutritionRequirementsQuery
	{
		/// <summary>
		/// Identyfikator użytkownika, dla którego mają zostać obliczone wymagania żywieniowe.
		/// </summary>
		public string UserId { get; set; } = null!;

		/// <summary>
		/// Inicjalizuje nowe zapytanie o wymagania żywieniowe.
		/// </summary>
		/// <param name="userId">Identyfikator użytkownika.</param>
		public GetDailyNutritionRequirementsQuery(string userId)
		{
			UserId = userId;
		}

		/// <summary>
		/// Konstruktor bezparametrowy wymagany przez AutoMapper i serializację.
		/// </summary>
		public GetDailyNutritionRequirementsQuery() { }
	}
}