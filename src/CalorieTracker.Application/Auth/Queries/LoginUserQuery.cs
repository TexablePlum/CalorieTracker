using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalorieTracker.Application.Auth.Queries
{
    public class LoginUserQuery
    {
		public string Email { get; init; } = null!;
		public string Password { get; init; } = null!;
	}
}
