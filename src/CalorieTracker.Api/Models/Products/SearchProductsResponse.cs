namespace CalorieTracker.Api.Models.Products
{
	/// <summary>
	/// Response dla wyszukiwania z paginacją
	/// </summary>
	public class SearchProductsResponse
	{
		public List<ProductSummaryDto> Products { get; set; } = new();
		public int TotalCount { get; set; }
		public bool HasMore { get; set; }
	}
}
