using CalorieTracker.Application.Auth.Interfaces;
using CalorieTracker.Application.Auth.Queries;
using CalorieTracker.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalorieTracker.Application.Auth.Handlers
{
    public class LoginUserHandler
    {
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IJwtGenerator _jwtGenerator;

		public LoginUserHandler(UserManager<ApplicationUser> userManager, IJwtGenerator jwtGenerator)
		{
			_userManager = userManager;
			_jwtGenerator = jwtGenerator;
		}

		public async Task<string?> Handle(LoginUserQuery query)
		{
			var user = await _userManager.FindByEmailAsync(query.Email);
			if (user is null) return null;

			var valid = await _userManager.CheckPasswordAsync(user, query.Password);
			if (!valid) return null;

			return _jwtGenerator.CreateToken(user);
		}
	}
}
