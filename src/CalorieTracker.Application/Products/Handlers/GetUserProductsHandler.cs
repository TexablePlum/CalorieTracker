using CalorieTracker.Application.Auth.Queries;
using CalorieTracker.Application.Interfaces;
using CalorieTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.Auth.Handlers
{
	/// <summary>
	/// Handler do pobierania produktów użytkownika
	/// </summary>
	public class GetUserProductsHandler
	{
		private readonly IAppDbContext _db;

		public GetUserProductsHandler(IAppDbContext db) => _db = db;

		public async Task<List<Product>> Handle(GetUserProductsQuery query)
		{
			return await _db.Products
				.Where(p => p.CreatedByUserId == query.UserId)
				.OrderByDescending(p => p.CreatedAt)
				.Skip(query.Skip)
				.Take(query.Take)
				.ToListAsync();
		}
	}
}
