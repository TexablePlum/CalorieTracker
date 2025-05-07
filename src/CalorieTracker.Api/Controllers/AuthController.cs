using AutoMapper;
using CalorieTracker.Api.Models.Auth;
using CalorieTracker.Application.Auth.Commands;
using CalorieTracker.Application.Auth.Handlers;
using CalorieTracker.Application.Auth.Interfaces;
using CalorieTracker.Application.Auth.Queries;
using CalorieTracker.Domain.Entities;
using CalorieTracker.Infrastructure.Email;                   
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;                        
using System.Security.Claims;

namespace CalorieTracker.Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		// handlery
		private readonly RegisterUserHandler _register;
		private readonly LoginUserHandler _login;
		private readonly UseRefreshTokenHandler _useRefresh;
		private readonly LogoutHandler _logout;
		// narzędzia 
		private readonly SignInManager<ApplicationUser> _signIn;
		private readonly IEmailSender _mail;             
		private readonly EmailSettings _mailCfg;          
		private readonly IMapper _mapper;

		public AuthController(
			RegisterUserHandler register,
			LoginUserHandler login,
			UseRefreshTokenHandler useRefresh,
			LogoutHandler logout,
			SignInManager<ApplicationUser> signIn,
			IEmailSender mail,                 
			IOptions<EmailSettings> mailCfg,              
			IMapper mapper)
		{
			_register = register;
			_login = login;
			_useRefresh = useRefresh;
			_logout = logout;
			_signIn = signIn;
			_mail = mail;                            
			_mailCfg = mailCfg.Value;                   
			_mapper = mapper;
		}

		// ---------- REGISTER ----------
		[HttpPost("register")]
		public async Task<IActionResult> Register(RegisterRequest request)
		{
			var cmd = _mapper.Map<RegisterUserCommand>(request);
			var result = await _register.Handle(cmd);

			if (!result.Succeeded &&
				result.Errors.Any(e => e.Code is "DuplicateUserName" or "DuplicateEmail"))
				return Conflict("Użytkownik z takim e-mailem już istnieje.");

			if (!result.Succeeded)
				return BadRequest(result.Errors.Select(e => e.Description));

			// Wysyłka linku potwierdzającego
			var user = await _signIn.UserManager.FindByEmailAsync(request.Email);

			if (user is not null)
			{
				var token = await _signIn.UserManager.GenerateEmailConfirmationTokenAsync(user);
				var link = $"{_mailCfg.ConfirmUrl}?userId={user.Id}&token={Uri.EscapeDataString(token)}";

				var html = $"<p>Hello {user.FirstName ?? user.Email}!</p>" +
						   $"<p>Please confirm your CalorieTracker account by clicking " +
						   $"<a href=\"{link}\">this link</a>.</p>";

				await _mail.SendAsync(user.Email!, "Confirm your CalorieTracker account", html);
			}

			return Ok();
		}

		// ---------- LOGIN ----------
		[HttpPost("login")]
		public async Task<IActionResult> Login(LoginRequest request)
		{
			var query = _mapper.Map<LoginUserQuery>(request);
			var tokens = await _login.Handle(query);

			if (tokens is null)
				return Unauthorized("Nieprawidłowy e-mail, hasło lub e-mail niepotwierdzony.");

			return Ok(new AuthResponse
			{
				AccessToken = tokens.Value.access,
				RefreshToken = tokens.Value.refresh,
				ExpiresAt = DateTime.UtcNow.AddHours(3)
			});
		}

		// ---------- CONFIRM ----------
		/// <summary>Kończy proces rejestracji – aktywuje konto.</summary>
		[AllowAnonymous]
		[HttpGet("confirm")]
		public async Task<IActionResult> Confirm(string userId, string token)
		{
			var user = await _signIn.UserManager.FindByIdAsync(userId);
			if (user is null) return NotFound("User not found");

			var res = await _signIn.UserManager.ConfirmEmailAsync(user, token);
			return res.Succeeded
				? Ok("E-mail potwierdzony! Możesz się zalogować.")
				: BadRequest(res.Errors.Select(e => e.Description));
		}

		// ---------- REFRESH ----------
		[HttpPost("refresh")]
		public async Task<IActionResult> Refresh(RefreshRequest req)
		{
			var (access, refresh) = await _useRefresh.Handle(
										new UseRefreshTokenCommand(req.AccessToken, req.RefreshToken));

			if (access is null)
				return Unauthorized("Nieprawidłowy lub wygasły refresh token.");

			return Ok(new
			{
				accessToken = access,
				refreshToken = refresh,
				expiresAt = DateTime.UtcNow.AddHours(3)
			});
		}

		// ---------- LOGOUT ----------
		[Authorize]
		[HttpPost("logout")]
		public async Task<IActionResult> Logout(LogoutRequest req)
		{
			await _logout.Handle(new LogoutCommand(req.RefreshToken));
			await _signIn.SignOutAsync();
			return Ok();
		}

		// ---------- TEST ----------
		[Authorize]
		[HttpGet("test")]
		public IActionResult TestAuth()
		{
			var email = User.FindFirstValue(ClaimTypes.Email);
			return Ok(new { message = "Autoryzacja przebiegła pomyślnie!", email });
		}
	}
}
