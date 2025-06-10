using CalorieTracker.Application.Auth.Queries;
using CalorieTracker.Application.Interfaces;
using CalorieTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Application.Auth.Handlers
{
	/// <summary>
	/// Handler do pobierania produktu po kodzie kreskowym
	/// </summary>
	public class GetProductByBarcodeHandler
	{
		private readonly IAppDbContext _db;

		public GetProductByBarcodeHandler(IAppDbContext db) => _db = db;

		public async Task<Product?> Handle(GetProductByBarcodeQuery query)
		{
			return await _db.Products
				.FirstOrDefaultAsync(p => p.Barcode == query.Barcode);
		}
	}
}
