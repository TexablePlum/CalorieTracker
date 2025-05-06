using System;

namespace CalorieTracker.Domain.Entities;

public class RefreshToken
{
	public Guid Id { get; set; }
	public string Token { get; set; } = Guid.NewGuid().ToString("N");
	public DateTime ExpiresAt { get; set; }
	public bool Revoked { get; set; }

	// relacja 1‑N z ApplicationUser
	public string UserId { get; set; } = null!;
	public ApplicationUser User { get; set; } = null!;
}
