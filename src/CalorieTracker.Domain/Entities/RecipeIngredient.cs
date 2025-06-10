using System.ComponentModel.DataAnnotations;

namespace CalorieTracker.Domain.Entities
{
	/// <summary>
	/// Encja reprezentująca składnik w przepisie kulinarnym
	/// </summary>
	public class RecipeIngredient
	{
		[Key]
		public Guid Id { get; set; } = Guid.NewGuid();

		/// <summary>ID przepisu do którego należy składnik</summary>
		[Required]
		public Guid RecipeId { get; set; }

		/// <summary>ID produktu używanego jako składnik</summary>
		[Required]
		public Guid ProductId { get; set; }

		/// <summary>Ilość produktu w jednostce zdefiniowanej w Product.Unit</summary>
		[Required]
		[Range(0.1, 10000)]
		public float Quantity { get; set; }

		// === RELACJE ===

		/// <summary>Przepis do którego należy składnik</summary>
		public Recipe Recipe { get; set; } = null!;

		/// <summary>Produkt używany jako składnik</summary>
		public Product Product { get; set; } = null!;
	}
}