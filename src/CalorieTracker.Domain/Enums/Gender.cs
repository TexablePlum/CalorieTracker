// Plik Gender.cs - definicja typu wyliczeniowego dla płci.
// Używany do określenia płci użytkownika w profilu.

namespace CalorieTracker.Domain.Enums
{
	/// &lt;summary>
	/// Definiuje dostępne opcje płci.
	/// Jest wykorzystywany w &lt;see cref="UserProfile"/> do kategoryzacji użytkowników.
	/// &lt;/summary>
	public enum Gender
	{
		/// &lt;summary>
		/// Oznacza płeć męską.
		/// &lt;/summary>
		Male,

		/// <summary>
		/// Oznacza płeć żeńską.
		/// </summary>
		Female
	}
}