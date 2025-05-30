namespace CalorieTracker.Application.Auth.Commands
{
	/// <summary>
	/// Komenda do usunięcia produktu
	/// </summary>
	public record DeleteProductCommand(Guid Id, string DeletedByUserId);
}
