using CalorieTracker.Application.Auth.Commands;
using CalorieTracker.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CalorieTracker.Application.Auth.Handlers
{
    public class RegisterUserHandler
    {
		private readonly UserManager<ApplicationUser> _userManager;
		public RegisterUserHandler(UserManager<ApplicationUser> userManager)
			=> _userManager = userManager;

		public async Task<IdentityResult> Handle(RegisterUserCommand cmd)
		{
			var user = new ApplicationUser
			{
				UserName = cmd.Email,
				Email = cmd.Email,
				FirstName = cmd.FirstName,
				LastName = cmd.LastName
			};
			return await _userManager.CreateAsync(user, cmd.Password);
		}
	}
}
