using CalorieTracker.Domain.Enums;

namespace CalorieTracker.Domain.Entities
{
    public class UserProfile
    {
		public Guid Id { get; set; } = Guid.NewGuid();

		// Podstawowe dane użytkownika
		public GoalType? Goal { get; set; }
		public Gender? Gender { get; set; }
		public int? Age { get; set; }
		public float? HeightCm { get; set; }
		public float? WeightKg { get; set; }
		public float? TargetWeightKg { get; set; }
		public ActivityLevel? ActivityLevel { get; set; }
		public float? WeeklyGoalChangeKg { get; set; }
		public bool[] MealPlan { get; set; } = new bool[6]; // śniadanie, 2 śniadanie, lunch, obiad, przekąska, kolacja


		// Relacja 1:1 z User
		public string UserId { get; set; } = null!;
		public ApplicationUser User { get; set; } = null!;
	}
}
