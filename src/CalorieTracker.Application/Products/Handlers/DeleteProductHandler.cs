using CalorieTracker.Application.Auth.Commands;
using CalorieTracker.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.Auth.Handlers
{
	/// <summary>
	/// Handler do usuwania produktu
	/// </summary>
	public class DeleteProductHandler
	{
		private readonly IAppDbContext _db;

		public DeleteProductHandler(IAppDbContext db) => _db = db;

		public async Task<bool> Handle(DeleteProductCommand command)
		{
			var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == command.Id);
			if (product is null) return false;

			// Sprawdzenie uprawnień
			if (product.CreatedByUserId != command.DeletedByUserId && !await IsAdmin(command.DeletedByUserId))
				return false;

			_db.Products.Remove(product);
			await _db.SaveChangesAsync();

			return true;
		}

		private Task<bool> IsAdmin(string userId)
		{
			return Task.FromResult(false); // TODO: Może kiedyś jak będą role użytkowników
		}
	}
}
