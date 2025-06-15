namespace CalorieTracker.Api.Models.Recipes
{
	/// <summary>
	/// Model transferu danych (DTO) reprezentujący składnik w przepisie.
	/// Zawiera szczegóły produktu oraz informacje o użytej ilości.
	/// </summary>
	public class RecipeIngredientDto
	{
		/// <summary>
		/// Unikalny identyfikator produktu w systemie.
		/// </summary>
		public Guid ProductId { get; set; }

		/// <summary>
		/// Nazwa produktu użytego jako składnik. Pole wymagane.
		/// </summary>
		public string ProductName { get; set; } = null!;

		/// <summary>
		/// Kategoria produktu (np. nabiał, warzywa). Pole wymagane.
		/// </summary>
		public string Category { get; set; } = null!;

		/// <summary>
		/// Ilość produktu użyta w przepisie.
		/// </summary>
		public float Quantity { get; set; }

		/// <summary>
		/// Jednostka miary dla ilości produktu (np. gramy, sztuki). Pole wymagane.
		/// </summary>
		public string Unit { get; set; } = null!;
	}
}
