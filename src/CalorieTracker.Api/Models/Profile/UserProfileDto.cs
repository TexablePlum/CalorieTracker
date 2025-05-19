namespace CalorieTracker.Api.Models.Profile
{
	public class UserProfileDto
	{
		// flaga kompletności profilu
		public bool IsComplete { get; set; }

		// dane z ApplicationUser
		public string Email { get; set; } = null!;
		public string? FirstName { get; set; }
		public string? LastName { get; set; }

		// dane z UserProfile
		public string? Goal { get; set; }
		public string? Gender { get; set; }
		public int? Age { get; set; }
		public float? HeightCm { get; set; }
		public float? WeightKg { get; set; }
		public float? TargetWeightKg { get; set; }
		public string? ActivityLevel { get; set; }
		public float? WeeklyGoalChangeKg { get; set; }
		public List<bool>? MealPlan { get; set; }
	}
}
