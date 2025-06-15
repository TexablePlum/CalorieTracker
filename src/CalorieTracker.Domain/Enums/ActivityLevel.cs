// Plik ActivityLevel.cs - definicja typu wyliczeniowego dla poziomu aktywności fizycznej.
// Służy do kategoryzacji aktywności użytkownika, co wpływa na zapotrzebowanie kaloryczne.

namespace CalorieTracker.Domain.Enums
{
	/// &lt;summary>
	/// Definiuje różne poziomy aktywności fizycznej użytkownika.
	/// Poziomy te są wykorzystywane do szacowania dziennego zapotrzebowania kalorycznego.
	/// &lt;/summary>
	public enum ActivityLevel
	{
		/// &lt;summary>
		/// Oznacza bardzo małą lub brak aktywności fizycznej (np. praca siedząca, brak ćwiczeń).
		/// &lt;/summary>
		Sedentary,

		/// <summary>
		/// Oznacza lekką aktywność fizyczną (np. lekkie ćwiczenia 1-3 razy w tygodniu).
		/// </summary>
		LightlyActive,

		/// <summary>
		/// Oznacza umiarkowaną aktywność fizyczną (np. umiarkowane ćwiczenia 3-5 razy w tygodniu).
		/// </summary>
		ModeratelyActive,

		/// <summary>
		/// Oznacza wysoką aktywność fizyczną (np. intensywne ćwiczenia 6-7 razy w tygodniu).
		/// </summary>
		VeryActive,

		/// <summary>
		/// Oznacza bardzo wysoką aktywność fizyczną (np. bardzo ciężkie ćwiczenia, praca fizyczna, treningi dwa razy dziennie).
		/// </summary>
		ExtremelyActive
	}
}