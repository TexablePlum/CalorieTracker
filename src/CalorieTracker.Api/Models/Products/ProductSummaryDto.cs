namespace CalorieTracker.Api.Models.Products
{
	/// <summary>
	/// Uproszczone DTO produktu dla list wyszukiwania
	/// </summary>
	public class ProductSummaryDto
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = null!;
		public string? Brand { get; set; }
		public string Category { get; set; } = null!;
		public string Unit { get; set; } = null!;
		public float ServingSize { get; set; }
		public float CaloriesPer100g { get; set; }
		public bool IsVerified { get; set; }
	}
}
