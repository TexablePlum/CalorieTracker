namespace CalorieTracker.Api.Models.Auth
{
	public class ConfirmEmailRequest
	{
		public string Email { get; set; } = null!;
		public string Code { get; set; } = null!;
	}
}
