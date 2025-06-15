// Plik RecipeNutrition.cs – Value Object reprezentujący zestaw wartości odżywczych przepisu.
// Umożliwia sumowanie i skalowanie składników odżywczych na potrzeby analizy składników posiłków i dań.

namespace CalorieTracker.Domain.ValueObjects
{
	/// <summary>
	/// Value Object przechowujący informacje o wartościach odżywczych danego przepisu.
	/// Obejmuje kalorie, makroskładniki oraz opcjonalnie błonnik, cukier i sód.
	/// Może być wykorzystywany do sumowania lub skalowania danych dla porcji lub składników.
	/// </summary>
	public class RecipeNutrition
	{
		/// <summary>
		/// Ilość kalorii.
		/// </summary>
		public float Calories { get; set; }

		/// <summary>
		/// Ilość białka (g).
		/// </summary>
		public float Protein { get; set; }

		/// <summary>
		/// Ilość tłuszczu (g).
		/// </summary>
		public float Fat { get; set; }

		/// <summary>
		/// Ilość węglowodanów (g).
		/// </summary>
		public float Carbohydrates { get; set; }

		/// <summary>
		/// Ilość błonnika (g), opcjonalna.
		/// </summary>
		public float? Fiber { get; set; }

		/// <summary>
		/// Ilość cukru (g), opcjonalna.
		/// </summary>
		public float? Sugar { get; set; }

		/// <summary>
		/// Ilość sodu (mg), opcjonalna.
		/// </summary>
		public float? Sodium { get; set; }

		/// <summary>
		/// Konstruktor domyślny – wymagany m.in. przez serializację.
		/// </summary>
		public RecipeNutrition()
		{
		}

		/// <summary>
		/// Tworzy obiekt wartości odżywczych na podstawie podanych składników.
		/// </summary>
		/// <param name="calories">Kalorie.</param>
		/// <param name="protein">Białko.</param>
		/// <param name="fat">Tłuszcz.</param>
		/// <param name="carbohydrates">Węglowodany.</param>
		/// <param name="fiber">Błonnik (opcjonalnie).</param>
		/// <param name="sugar">Cukier (opcjonalnie).</param>
		/// <param name="sodium">Sód (opcjonalnie).</param>
		public RecipeNutrition(float calories, float protein, float fat, float carbohydrates,
			float? fiber = null, float? sugar = null, float? sodium = null)
		{
			Calories = calories;
			Protein = protein;
			Fat = fat;
			Carbohydrates = carbohydrates;
			Fiber = fiber;
			Sugar = sugar;
			Sodium = sodium;
		}

		/// <summary>
		/// Dodaje wartości odżywcze z innego obiektu do bieżącego.
		/// Wartości opcjonalne (błonnik, cukier, sód) są sumowane tylko jeśli są dostępne.
		/// </summary>
		/// <param name="other">Obiekt zawierający dodatkowe wartości do dodania.</param>
		public void Add(RecipeNutrition other)
		{
			Calories += other.Calories;
			Protein += other.Protein;
			Fat += other.Fat;
			Carbohydrates += other.Carbohydrates;

			if (other.Fiber.HasValue)
				Fiber = (Fiber ?? 0) + other.Fiber.Value;

			if (other.Sugar.HasValue)
				Sugar = (Sugar ?? 0) + other.Sugar.Value;

			if (other.Sodium.HasValue)
				Sodium = (Sodium ?? 0) + other.Sodium.Value;
		}

		/// <summary>
		/// Tworzy nowy obiekt, w którym wszystkie składniki zostały przeskalowane przez podany współczynnik.
		/// </summary>
		/// <param name="factor">Współczynnik skalowania (np. 0.5 dla połowy porcji).</param>
		/// <returns>Nowy obiekt z przeskalowanymi wartościami odżywczymi.</returns>
		public RecipeNutrition Scale(float factor)
		{
			return new RecipeNutrition(
				Calories * factor,
				Protein * factor,
				Fat * factor,
				Carbohydrates * factor,
				Fiber * factor,
				Sugar * factor,
				Sodium * factor
			);
		}
	}
}
