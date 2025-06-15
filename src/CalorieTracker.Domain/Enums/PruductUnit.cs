// Plik ProductUnit.cs - definicja typu wyliczeniowego dla jednostek miary produktów spożywczych.
// Określa standardowe jednostki używane do ilościowego opisywania produktów.

namespace CalorieTracker.Domain.Enums
{
	/// &lt;summary>
	/// Jednostki miary produktów spożywczych.
	/// Używane do określenia sposobu mierzenia &lt;see cref="Product">produktów&lt;/see>.
	/// &lt;/summary>
	public enum ProductUnit
	{
		/// &lt;summary>
		/// Gramy - podstawowa jednostka miary dla większości stałych produktów spożywczych.
		/// &lt;/summary>
		Gram,

		/// <summary>
		/// Mililitry - jednostka miary przeznaczona głównie dla płynów.
		/// </summary>
		Milliliter,

		/// <summary>
		/// Sztuki - jednostka miary dla produktów liczonych indywidualnie (np. jajka, bułki).
		/// </summary>
		Piece
	}
}