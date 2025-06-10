using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalorieTracker.Application.Recipes.Queries
{
	/// <summary>
	/// Query do wyszukiwania przepisów
	/// </summary>
	public class SearchRecipesQuery
	{
		public string SearchTerm { get; init; } = null!;
		public int Skip { get; init; } = 0;
		public int Take { get; init; } = 20;
	}
}
