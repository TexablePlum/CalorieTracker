// Plik SearchProductsRequest.cs - model żądania wyszukiwania produktów.
// Odpowiada za przechwytywanie parametrów wyszukiwania i filtrowania listy produktów.

namespace CalorieTracker.Api.Models.Products
{
	/// <summary>
	/// Model żądania wyszukiwania i stronicowania produktów.
	/// Umożliwia filtrowanie produktów po nazwie, kategorii oraz statusie weryfikacji.
	/// </summary>
	public class SearchProductsRequest
	{
		/// <summary>
		/// Fraza wyszukiwana w nazwie i opisie produktu. Pole wymagane.
		/// </summary>
		public string SearchTerm { get; set; } = null!;

		/// <summary>
		/// Kategoria produktu do filtrowania wyników (opcjonalne).
		/// </summary>
		public string? Category { get; set; }

		/// <summary>
		/// Liczba produktów do pominięcia (stronicowanie). Domyślnie 0.
		/// </summary>
		public int Skip { get; set; } = 0;

		/// <summary>
		/// Maksymalna liczba produktów do zwrócenia. Domyślnie 20.
		/// </summary>
		public int Take { get; set; } = 20;

		/// <summary>
		/// Flaga określająca czy zwracać tylko zweryfikowane produkty. Domyślnie false.
		/// </summary>
		public bool VerifiedOnly { get; set; } = false;
	}
}