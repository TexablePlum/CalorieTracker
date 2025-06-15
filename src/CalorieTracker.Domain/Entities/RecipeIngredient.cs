// Plik RecipeIngredient.cs - definicja encji składnika przepisu.
// Reprezentuje konkretny produkt użyty w danym przepisie kulinarnym wraz z jego ilością.

using System.ComponentModel.DataAnnotations;

namespace CalorieTracker.Domain.Entities
{
	/// &lt;summary>
	/// Encja reprezentująca składnik w przepisie kulinarnym.
	/// Definiuje jaki &lt;see cref="Product"/> jest używany w danym &lt;see cref="Recipe"/>
	/// oraz w jakiej &lt;see cref="Quantity">ilości&lt;/see>.
	/// &lt;/summary>
	public class RecipeIngredient
	{
		/// &lt;summary>
		/// Unikalny identyfikator GUID składnika przepisu.
		/// Jest kluczem głównym encji.
		/// &lt;/summary>
		[Key]
		public Guid Id { get; set; } = Guid.NewGuid();

		/// <summary>
		/// Identyfikator GUID przepisu, do którego należy ten składnik.
		/// Pole wymagane.
		/// </summary>
		[Required]
		public Guid RecipeId { get; set; }

		/// <summary>
		/// Identyfikator GUID produktu używanego jako składnik.
		/// Pole wymagane.
		/// </summary>
		[Required]
		public Guid ProductId { get; set; }

		/// <summary>
		/// Ilość produktu w jednostce zdefiniowanej w <see cref="Product.Unit"/>.
		/// Pole wymagane, wartość w zakresie od 0.1 do 10000.
		/// </summary>
		[Required]
		[Range(0.1, 10000)]
		public float Quantity { get; set; }

		// === RELACJE ===

		/// <summary>
		/// Właściwość nawigacyjna do przepisu <see cref="Recipe"/>, do którego należy ten składnik.
		/// </summary>
		public Recipe Recipe { get; set; } = null!;

		/// <summary>
		/// Właściwość nawigacyjna do produktu <see cref="Product"/> używanego jako składnik.
		/// </summary>
		public Product Product { get; set; } = null!;
	}
}