using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalorieTracker.Application.Auth.Queries
{
	/// <summary>
	/// Query do pobrania produktów użytkownika
	/// </summary>
	public class GetUserProductsQuery
	{
		public string UserId { get; init; } = null!;
		public int Skip { get; init; } = 0;
		public int Take { get; init; } = 20;
	}
}
