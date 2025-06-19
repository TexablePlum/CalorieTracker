// Plik DailyNutritionRequirements.cs - Value Object reprezentujący dzienne zapotrzebowanie żywieniowe.
// Hermetyzuje wyniki kalkulacji dziennych potrzeb kalorycznych, makroskładników i wody.

namespace CalorieTracker.Domain.ValueObjects
{
	/// <summary>
	/// Value Object przechowujący kompletne dzienne zapotrzebowanie żywieniowe użytkownika.
	/// Zawiera obliczone kalorie, makroskładniki w przedziałach oraz zapotrzebowanie na wodę.
	/// Immutable object zapewniający spójność danych odżywczych.
	/// </summary>
	public class DailyNutritionRequirements
	{
		/// <summary>
		/// Dzienne zapotrzebowanie kaloryczne w kcal.
		/// Obliczone na podstawie BMR i TDEE z uwzględnieniem celu wagowego.
		/// </summary>
		public float Calories { get; }

		/// <summary>
		/// Minimalne dzienne zapotrzebowanie na białko w gramach.
		/// Obliczone według wytycznych 0.8-1.2g/kg masy ciała.
		/// </summary>
		public float ProteinMinGrams { get; }

		/// <summary>
		/// Maksymalne dzienne zapotrzebowanie na białko w gramach.
		/// Uwzględnia cel użytkownika i poziom aktywności.
		/// </summary>
		public float ProteinMaxGrams { get; }

		/// <summary>
		/// Minimalne dzienne zapotrzebowanie na tłuszcze w gramach.
		/// Obliczone jako 20-25% dziennej podaży kalorii.
		/// </summary>
		public float FatMinGrams { get; }

		/// <summary>
		/// Maksymalne dzienne zapotrzebowanie na tłuszcze w gramach.
		/// Uwzględnia cel użytkownika i preferencje żywieniowe.
		/// </summary>
		public float FatMaxGrams { get; }

		/// <summary>
		/// Minimalne dzienne zapotrzebowanie na węglowodany w gramach.
		/// Obliczone jako pozostałość po białku i tłuszczach.
		/// </summary>
		public float CarbohydratesMinGrams { get; }

		/// <summary>
		/// Maksymalne dzienne zapotrzebowanie na węglowodany w gramach.
		/// Dostosowane do celu wagowego i preferencji dietetycznych.
		/// </summary>
		public float CarbohydratesMaxGrams { get; }

		/// <summary>
		/// Dzienne zapotrzebowanie na wodę w litrach.
		/// Obliczone według zaleceń IOM z uwzględnieniem masy ciała i aktywności.
		/// </summary>
		public float WaterLiters { get; }

		/// <summary>
		/// Podstawowy wskaźnik metabolizmu (BMR) w kcal.
		/// Energia potrzebna do podstawowych funkcji życiowych.
		/// </summary>
		public float BMR { get; }

		/// <summary>
		/// Całkowity wydatek energetyczny (TDEE) w kcal.
		/// BMR pomnożony przez współczynnik aktywności fizycznej.
		/// </summary>
		public float TDEE { get; }

		/// <summary>
		/// Tworzy nową instancję dziennego zapotrzebowania żywieniowego.
		/// </summary>
		/// <param name="calories">Dzienne zapotrzebowanie kaloryczne.</param>
		/// <param name="proteinMinGrams">Minimalne zapotrzebowanie na białko.</param>
		/// <param name="proteinMaxGrams">Maksymalne zapotrzebowanie na białko.</param>
		/// <param name="fatMinGrams">Minimalne zapotrzebowanie na tłuszcze.</param>
		/// <param name="fatMaxGrams">Maksymalne zapotrzebowanie na tłuszcze.</param>
		/// <param name="carbohydratesMinGrams">Minimalne zapotrzebowanie na węglowodany.</param>
		/// <param name="carbohydratesMaxGrams">Maksymalne zapotrzebowanie na węglowodany.</param>
		/// <param name="waterLiters">Zapotrzebowanie na wodę.</param>
		/// <param name="bmr">Podstawowy wskaźnik metabolizmu.</param>
		/// <param name="tdee">Całkowity wydatek energetyczny.</param>
		public DailyNutritionRequirements(
			float calories,
			float proteinMinGrams,
			float proteinMaxGrams,
			float fatMinGrams,
			float fatMaxGrams,
			float carbohydratesMinGrams,
			float carbohydratesMaxGrams,
			float waterLiters,
			float bmr,
			float tdee)
		{
			Calories = calories;
			ProteinMinGrams = proteinMinGrams;
			ProteinMaxGrams = proteinMaxGrams;
			FatMinGrams = fatMinGrams;
			FatMaxGrams = fatMaxGrams;
			CarbohydratesMinGrams = carbohydratesMinGrams;
			CarbohydratesMaxGrams = carbohydratesMaxGrams;
			WaterLiters = waterLiters;
			BMR = bmr;
			TDEE = tdee;
		}

		/// <summary>
		/// Zwraca środkową wartość zapotrzebowania na białko.
		/// Przydatne do szybkich kalkulacji i wyświetlania.
		/// </summary>
		public float ProteinAverageGrams => (ProteinMinGrams + ProteinMaxGrams) / 2f;

		/// <summary>
		/// Zwraca środkową wartość zapotrzebowania na tłuszcze.
		/// Przydatne do szybkich kalkulacji i wyświetlania.
		/// </summary>
		public float FatAverageGrams => (FatMinGrams + FatMaxGrams) / 2f;

		/// <summary>
		/// Zwraca środkową wartość zapotrzebowania na węglowodany.
		/// Przydatne do szybkich kalkulacji i wyświetlania.
		/// </summary>
		public float CarbohydratesAverageGrams => (CarbohydratesMinGrams + CarbohydratesMaxGrams) / 2f;
	}
}