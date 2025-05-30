using CalorieTracker.Application.Interfaces;
using CalorieTracker.Application.Products.Commands;
using CalorieTracker.Domain.Entities;

namespace CalorieTracker.Application.Auth.Handlers
{
	/// <summary>
	/// Handler do tworzenia nowego produktu
	/// </summary>
	public class CreateProductHandler
	{
		private readonly IAppDbContext _db;

		public CreateProductHandler(IAppDbContext db) => _db = db;

		public async Task<Guid> Handle(CreateProductCommand command)
		{
			var product = new Product
			{
				Name = command.Name,
				Brand = command.Brand,
				Description = command.Description,
				Ingredients = command.Ingredients,
				Barcode = command.Barcode,
				Category = command.Category,
				Unit = command.Unit,
				ServingSize = command.ServingSize,
				CaloriesPer100g = command.CaloriesPer100g,
				ProteinPer100g = command.ProteinPer100g,
				FatPer100g = command.FatPer100g,
				CarbohydratesPer100g = command.CarbohydratesPer100g,
				FiberPer100g = command.FiberPer100g,
				SugarsPer100g = command.SugarsPer100g,
				SodiumPer100g = command.SodiumPer100g,
				CreatedByUserId = command.CreatedByUserId,
				CreatedAt = DateTime.UtcNow,
				UpdatedAt = DateTime.UtcNow
			};

			_db.Products.Add(product);
			await _db.SaveChangesAsync();

			return product.Id;
		}
	}
}
