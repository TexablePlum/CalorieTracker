using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalorieTracker.Application.Auth.Commands
{
    public class RegisterUserCommand
    {
		public string Email { get; init; } = null!;
		public string Password { get; init; } = null!;
		public string? FirstName { get; init; }
		public string? LastName { get; init; }
	}
}
