using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalorieTracker.Application.Recipes.Commands
{
	/// <summary>
	/// Komenda do usunięcia przepisu
	/// </summary>
	public record DeleteRecipeCommand(Guid Id, string DeletedByUserId);
}
