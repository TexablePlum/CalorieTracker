using CalorieTracker.Domain.Enums;

namespace CalorieTracker.Application.Products.Commands
{
	/// <summary>
	/// Komenda do utworzenia nowego produktu
	/// </summary>
	public record CreateProductCommand
	{
		public string Name { get; init; } = null!;
		public string? Brand { get; init; }
		public string? Description { get; init; }
		public string? Ingredients { get; init; }
		public string? Barcode { get; init; }
		public ProductCategory Category { get; init; }
		public ProductUnit Unit { get; init; }
		public float ServingSize { get; init; }
		public float CaloriesPer100g { get; init; }
		public float ProteinPer100g { get; init; }
		public float FatPer100g { get; init; }
		public float CarbohydratesPer100g { get; init; }
		public float? FiberPer100g { get; init; }
		public float? SugarsPer100g { get; init; }
		public float? SodiumPer100g { get; init; }
		public string CreatedByUserId { get; init; } = null!;
	}
}