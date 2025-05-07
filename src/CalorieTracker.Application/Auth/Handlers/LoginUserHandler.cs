using CalorieTracker.Application.Auth.Commands;
using CalorieTracker.Application.Auth.Interfaces;
using CalorieTracker.Application.Auth.Queries;
using CalorieTracker.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CalorieTracker.Application.Auth.Handlers
{
	/// <summary>
	/// Logowanie użytkownika: walidacja hasła + generacja JWT + refresh token.
	/// </summary>
	public class LoginUserHandler
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly IJwtGenerator _jwtGenerator;
		private readonly GenerateRefreshTokenHandler _generateRt;

		public LoginUserHandler(
			UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager,
			IJwtGenerator jwtGenerator,
			GenerateRefreshTokenHandler generateRt)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_jwtGenerator = jwtGenerator;
			_generateRt = generateRt;
		}

		/// <returns>null, gdy błędne dane, lockout lub nie zweryfikowany email, w przeciwnym razie krotka access/refresh tokens</returns>
		public async Task<(string access, string refresh)?> Handle(LoginUserQuery query)
		{
			var user = await _userManager.FindByEmailAsync(query.Email);
			if (user is null) return null;

			// Przymus potwierdzanie maila
			if (!await _userManager.IsEmailConfirmedAsync(user))
				return null;

			// PasswordSignInAsync -> liczy nieudane próby i lockout
			var signInRes = await _signInManager.PasswordSignInAsync(
								user, query.Password, false, true);
			if (!signInRes.Succeeded) return null;

			// JWT + nowy refresh‑token
			var access = _jwtGenerator.CreateToken(user);
			var refresh = await _generateRt.Handle(
							  new GenerateRefreshTokenCommand(user.Id));

			return (access, refresh);
		}
	}
}
