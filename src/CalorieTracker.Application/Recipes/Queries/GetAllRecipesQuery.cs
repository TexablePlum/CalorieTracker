using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalorieTracker.Application.Recipes.Queries
{
	/// <summary>
	/// Query do pobierania wszystkich przepisów (globalna lista)
	/// </summary>
	public class GetAllRecipesQuery
	{
		public int Skip { get; init; } = 0;
		public int Take { get; init; } = 20;
	}
}
