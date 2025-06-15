// Plik GoalType.cs - definicja typu wyliczeniowego dla celu użytkownika.
// Określa główny cel, jaki użytkownik chce osiągnąć w kontekście masy ciała.

namespace CalorieTracker.Domain.Enums
{
	/// &lt;summary>
	/// Definiuje różne typy celów, które użytkownik może sobie wyznaczyć w aplikacji.
	/// Jest wykorzystywany w &lt;see cref="UserProfile"/> do określenia zamierzeń użytkownika.
	/// &lt;/summary>
	public enum GoalType
	{
		/// &lt;summary>
		/// Oznacza cel utraty masy ciała.
		/// &lt;/summary>
		LoseWeight,

		/// <summary>
		/// Oznacza cel utrzymania aktualnej masy ciała.
		/// </summary>
		Maintain,

		/// <summary>
		/// Oznacza cel przyrostu masy ciała.
		/// </summary>
		GainWeight
	}
}