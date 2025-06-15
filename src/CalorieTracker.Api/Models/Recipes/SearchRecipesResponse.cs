namespace CalorieTracker.Api.Models.Recipes
{
	/// <summary>
	/// Model odpowiedzi na zapytanie wyszukiwania przepisów.
	/// Zawiera wyniki wyszukiwania wraz z informacjami o paginacji.
	/// </summary>
	public class SearchRecipesResponse
	{
		/// <summary>
		/// Lista znalezionych przepisów w formie podsumowania.
		/// Domyślnie pusta lista.
		/// </summary>
		public List<RecipeSummaryDto> Recipes { get; set; } = new();

		/// <summary>
		/// Całkowita liczba przepisów pasujących do kryteriów wyszukiwania.
		/// </summary>
		public int TotalCount { get; set; }

		/// <summary>
		/// Flaga wskazująca, czy istnieją kolejne strony wyników.
		/// </summary>
		public bool HasMore { get; set; }
	}
}
