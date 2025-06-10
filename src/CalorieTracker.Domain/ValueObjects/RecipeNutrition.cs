namespace CalorieTracker.Domain.ValueObjects
{
	/// <summary>
	/// Value Object przechowujący wartości odżywcze przepisu
	/// </summary>
	public class RecipeNutrition
	{
		public float Calories { get; set; }
		public float Protein { get; set; }
		public float Fat { get; set; }
		public float Carbohydrates { get; set; }
		public float? Fiber { get; set; }
		public float? Sugar { get; set; }
		public float? Sodium { get; set; }

		public RecipeNutrition()
		{
		}

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
		/// Dodaje wartości odżywcze do obecnych
		/// </summary>
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
		/// Przeskalowuje wartości odżywcze według podanego współczynnika
		/// </summary>
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