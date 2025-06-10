using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalorieTracker.Application.Recipes.Queries
{
	/// <summary>
	/// Query do pobierania przepisów użytkownika
	/// </summary>
	public class GetUserRecipesQuery
	{
		public string UserId { get; init; } = null!;
		public int Skip { get; init; } = 0;
		public int Take { get; init; } = 20;
	}
}
