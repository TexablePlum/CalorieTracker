
namespace CalorieTracker.Domain.Entities
{
	public class EmailConfirmation
	{
		public int Id { get; set; }
		public string UserId { get; set; } = null!;
		public string Code { get; set; } = null!;
		public DateTime ExpiresAt { get; set; }

		public ApplicationUser User { get; set; } = null!;
	}
}
