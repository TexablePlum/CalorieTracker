namespace CalorieTracker.Api.Models.Recipes
{
	/// <summary>
	/// Model transferu danych (DTO) do wyszukiwania przepisów.
	/// Umożliwia przeszukiwanie przepisów z paginacją wyników.
	/// </summary>
	public class SearchRecipesRequest
	{
		/// <summary>
		/// Fraza do wyszukania w nazwach i instrukcjach przepisów. Pole wymagane.
		/// </summary>
		public string SearchTerm { get; set; } = null!;

		/// <summary>
		/// Liczba przepisów do pominięcia (stronicowanie).
		/// Domyślnie 0.
		/// </summary>
		public int Skip { get; set; } = 0;

		/// <summary>
		/// Maksymalna liczba przepisów do zwrócenia.
		/// Domyślnie 20.
		/// </summary>
		public int Take { get; set; } = 20;
	}
}
