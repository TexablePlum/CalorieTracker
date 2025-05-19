using System.ComponentModel.DataAnnotations;

namespace CalorieTracker.Domain.Entities
{
	public class PasswordReset
	{
		[Key]
		public Guid Id { get; set; } = Guid.NewGuid();

		public string UserId { get; set; } = null!;
		public ApplicationUser User { get; set; } = null!;

		public string Code { get; set; } = null!;
		public DateTime ExpiresAt { get; set; }
	}
}
