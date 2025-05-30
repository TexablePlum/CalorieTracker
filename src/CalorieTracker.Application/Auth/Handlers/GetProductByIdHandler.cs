using CalorieTracker.Application.Auth.Queries;
using CalorieTracker.Application.Interfaces;
using CalorieTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.Auth.Handlers
{
	/// <summary>
	/// Handler do pobierania produktu po ID
	/// </summary>
	public class GetProductByIdHandler
	{
		private readonly IAppDbContext _db;

		public GetProductByIdHandler(IAppDbContext db) => _db = db;

		public async Task<Product?> Handle(GetProductByIdQuery query)
		{
			return await _db.Products
				.Include(p => p.CreatedByUser)
				.FirstOrDefaultAsync(p => p.Id == query.Id);
		}
	}
}
