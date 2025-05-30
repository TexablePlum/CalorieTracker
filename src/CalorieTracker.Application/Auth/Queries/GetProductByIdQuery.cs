using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalorieTracker.Application.Auth.Queries
{
	/// <summary>
	/// Query do pobrania produktu po ID
	/// </summary>
	public record GetProductByIdQuery(Guid Id);
}
