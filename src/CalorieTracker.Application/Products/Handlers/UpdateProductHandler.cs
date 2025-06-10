using CalorieTracker.Application.Auth.Commands;
using CalorieTracker.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalorieTracker.Application.Auth.Handlers
{
	/// <summary>
	/// Handler do aktualizacji produktu
	/// </summary>
	public class UpdateProductHandler
	{
		private readonly IAppDbContext _db;

		public UpdateProductHandler(IAppDbContext db) => _db = db;

		public async Task<bool> Handle(UpdateProductCommand command)
		{
			var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == command.Id);
			if (product is null) return false;

			// Sprawdzenie uprawnień - użytkownik może edytować tylko swoje produkty lub admin wszystkie
			if (product.CreatedByUserId != command.UpdatedByUserId && !await IsAdmin(command.UpdatedByUserId))
				return false;

			product.Name = command.Name;
			product.Brand = command.Brand;
			product.Description = command.Description;
			product.Ingredients = command.Ingredients;
			product.Barcode = command.Barcode;
			product.Category = command.Category;
			product.Unit = command.Unit;
			product.ServingSize = command.ServingSize;
			product.CaloriesPer100g = command.CaloriesPer100g;
			product.ProteinPer100g = command.ProteinPer100g;
			product.FatPer100g = command.FatPer100g;
			product.CarbohydratesPer100g = command.CarbohydratesPer100g;
			product.FiberPer100g = command.FiberPer100g;
			product.SugarsPer100g = command.SugarsPer100g;
			product.SodiumPer100g = command.SodiumPer100g;
			product.UpdatedAt = DateTime.UtcNow;

			await _db.SaveChangesAsync();
			return true;
		}

		private async Task<bool> IsAdmin(string userId)
		{
			// Tu można dodać logikę sprawdzania ról administratora
			// Na razie nie ma adminów
			return false;
		}
	}
}
