namespace CalorieTracker.Api.Models.Recipes
{
	/// <summary>
	/// Model transferu danych (DTO) reprezentujący składnik przepisu.
	/// Używany podczas tworzenia lub aktualizacji przepisu.
	/// </summary>
	public class CreateRecipeIngredientRequest
	{
		/// <summary>
		/// Identyfikator produktu będącego składnikiem przepisu.
		/// </summary>
		public Guid ProductId { get; set; }

		/// <summary>
		/// Ilość produktu w przepisie.
		/// </summary>
		public float Quantity { get; set; }
	}
}
