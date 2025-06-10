using CalorieTracker.Domain.Enums;

namespace CalorieTracker.Application.Auth.Queries
{
	/// <summary>
	/// Query do wyszukiwania produktów po nazwie
	/// </summary>
	public class SearchProductsQuery
	{
		public string SearchTerm { get; init; } = null!;
		public ProductCategory? Category { get; init; }
		public int Skip { get; init; } = 0;
		public int Take { get; init; } = 20;
		public bool VerifiedOnly { get; init; } = false;
	}
}
