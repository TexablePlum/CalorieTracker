namespace CalorieTracker.Api.Models.Products
{
	/// <summary>
	/// DTO produktu zwracane przez API
	/// </summary>
	public class ProductDto
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = null!;
		public string? Brand { get; set; }
		public string? Description { get; set; }
		public string? Ingredients { get; set; }
		public string? Barcode { get; set; }
		public string Category { get; set; } = null!;
		public string Unit { get; set; } = null!;
		public float ServingSize { get; set; }
		public float CaloriesPer100g { get; set; }
		public float ProteinPer100g { get; set; }
		public float FatPer100g { get; set; }
		public float CarbohydratesPer100g { get; set; }
		public float? FiberPer100g { get; set; }
		public float? SugarsPer100g { get; set; }
		public float? SodiumPer100g { get; set; }
		public bool IsVerified { get; set; }
		public string? CreatedByUserId { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
	}
}
