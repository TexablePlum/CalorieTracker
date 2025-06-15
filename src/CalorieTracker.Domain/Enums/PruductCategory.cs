// Plik ProductCategory.cs - definicja typu wyliczeniowego dla kategorii produktów spożywczych.
// Służy do kategoryzowania produktów w celu ułatwienia ich organizacji i wyszukiwania.

namespace CalorieTracker.Domain.Enums
{
	/// &lt;summary>
	/// Kategorie produktów spożywczych dla lepszej organizacji.
	/// Używane do klasyfikowania encji &lt;see cref="Product"/>.
	/// &lt;/summary>
	public enum ProductCategory
	{
		/// &lt;summary>
		/// Owoce.
		/// &lt;/summary>
		Fruits,

		/// <summary>
		/// Warzywa.
		/// </summary>
		Vegetables,

		/// <summary>
		/// Mięso i drób.
		/// </summary>
		MeatAndPoultry,

		/// <summary>
		/// Ryby i owoce morza.
		/// </summary>
		FishAndSeafood,

		/// <summary>
		/// Nabiał.
		/// </summary>
		Dairy,

		/// <summary>
		/// Zboża i produkty zbożowe.
		/// </summary>
		GrainsAndCereals,

		/// <summary>
		/// Bakalie i orzechy.
		/// </summary>
		NutsAndDriedFruits,

		/// <summary>
		/// Słodycze.
		/// </summary>
		Sweets,

		/// <summary>
		/// Napoje.
		/// </summary>
		Beverages,

		/// <summary>
		/// Oleje i tłuszcze.
		/// </summary>
		OilsAndFats,

		/// <summary>
		/// Przyprawy i zioła.
		/// </summary>
		SpicesAndHerbs,

		/// <summary>
		/// Gotowe posiłki.
		/// </summary>
		ReadyMeals,

		/// <summary>
		/// Inne produkty spożywcze, które nie pasują do pozostałych kategorii.
		/// </summary>
		Other
	}
}