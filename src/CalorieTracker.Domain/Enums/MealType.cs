// Plik MealType.cs - enum typu posiłku w dzienniku żywieniowym.
// Definiuje kategorie czasowe posiłków dla lepszej organizacji dziennika.

namespace CalorieTracker.Domain.Enums
{
	/// <summary>
	/// Typ posiłku określający kategorię czasową spożycia w dzienniku żywieniowym.
	/// Używany do grupowania i analizy wzorców żywieniowych użytkownika.
	/// UWAGA: Kolejność enum odpowiada indeksom w UserProfile.MealPlan[]
	/// </summary>
	public enum MealType
	{
		/// <summary>
		/// Śniadanie - pierwszy posiłek dnia, zazwyczaj rano.
		/// Indeks 0 w MealPlan.
		/// </summary>
		Breakfast,

		/// <summary>
		/// Drugie śniadanie - przekąska przedpołudniowa.
		/// Indeks 1 w MealPlan.
		/// </summary>
		MorningSnack,

		/// <summary>
		/// Lunch - posiłek główny w środku dnia.
		/// Indeks 2 w MealPlan.
		/// </summary>
		Lunch,

		/// <summary>
		/// Obiad - główny posiłek wieczorny.
		/// Indeks 3 w MealPlan.
		/// </summary>
		Dinner,

		/// <summary>
		/// Przekąska popołudniowa.
		/// Indeks 4 w MealPlan.
		/// </summary>
		AfternoonSnack,

		/// <summary>
		/// Kolacja/przekąska wieczorna - ostatni posiłek dnia.
		/// Indeks 5 w MealPlan.
		/// </summary>
		EveningSnack
	}
}