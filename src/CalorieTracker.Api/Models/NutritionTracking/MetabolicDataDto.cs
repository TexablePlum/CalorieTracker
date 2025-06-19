// Plik MetabolicDataDto.cs - DTO wskaźników metabolicznych.

namespace CalorieTracker.Api.Models.NutritionTracking
{
	/// <summary>
	/// Model transferu danych zawierający podstawowe wskaźniki metaboliczne.
	/// </summary>
	public class MetabolicDataDto
	{
		/// <summary>
		/// Podstawowy wskaźnik metabolizmu (BMR) w kcal.
		/// </summary>
		public float BMR { get; set; }

		/// <summary>
		/// Całkowity wydatek energetyczny (TDEE) w kcal.
		/// </summary>
		public float TDEE { get; set; }
	}
}