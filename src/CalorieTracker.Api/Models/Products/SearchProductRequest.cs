namespace CalorieTracker.Api.Models.Products
{
	/// <summary>
	/// DTO do wyszukiwania produktów
	/// </summary>
	public class SearchProductsRequest
	{
		public string SearchTerm { get; set; } = null!;
		public string? Category { get; set; }
		public int Skip { get; set; } = 0;
		public int Take { get; set; } = 20;
		public bool VerifiedOnly { get; set; } = false;
	}
}
