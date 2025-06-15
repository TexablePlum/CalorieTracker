// Plik SearchProductsResponse.cs - model odpowiedzi na wyszukiwanie produktów.
// Odpowiada za zwracanie wyników wyszukiwania produktów z informacjami o paginacji.

namespace CalorieTracker.Api.Models.Products
{
	/// <summary>
	/// Model odpowiedzi zawierający wyniki wyszukiwania produktów z paginacją.
	/// Przenosi listę produktów wraz z metadanymi dotyczącymi dostępnych wyników.
	/// </summary>
	public class SearchProductsResponse
	{
		/// <summary>
		/// Lista produktów spełniających kryteria wyszukiwania.
		/// Zawiera obiekty typu <see cref="ProductSummaryDto"/> z podstawowymi informacjami o produktach.
		/// </summary>
		public List<ProductSummaryDto> Products { get; set; } = new();

		/// <summary>
		/// Całkowita liczba produktów spełniających kryteria wyszukiwania (niezależnie od paginacji).
		/// </summary>
		public int TotalCount { get; set; }

		/// <summary>
		/// Flaga wskazująca czy istnieją kolejne strony wyników.
		/// Wartość true oznacza, że dostępne są dodatkowe wyniki poza obecną stroną.
		/// </summary>
		public bool HasMore { get; set; }
	}
}