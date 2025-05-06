using AutoMapper;
using CalorieTracker.Api.Models.Auth;
using CalorieTracker.Application.Auth.Commands;
using CalorieTracker.Application.Auth.Handlers;
using CalorieTracker.Application.Auth.Queries;
using CalorieTracker.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CalorieTracker.Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly RegisterUserHandler _register;
		private readonly LoginUserHandler _login;
		private readonly UseRefreshTokenHandler _useRefresh;
		private readonly LogoutHandler _logout;
		private readonly SignInManager<ApplicationUser> _signIn;
		private readonly IMapper _mapper;

		public AuthController(
			RegisterUserHandler register,
			LoginUserHandler login,
			UseRefreshTokenHandler useRefresh,
			LogoutHandler logout,
			SignInManager<ApplicationUser> signIn,
			IMapper mapper)
		{
			_register = register;
			_login = login;
			_useRefresh = useRefresh;
			_logout = logout;
			_signIn = signIn;
			_mapper = mapper;
		}

		// REGISTER
		/// <summary>
		/// Tworzy nowe konto. Zwraca 200 OK lub 409 Conflict gdy mail zajęty.
		/// </summary>
		[HttpPost("register")]
		public async Task<IActionResult> Register(RegisterRequest request)
		{
			// mapowanie DTO -> Command (CQRS)
			var cmd = _mapper.Map<RegisterUserCommand>(request);
			var result = await _register.Handle(cmd);

			// 409 dla duplikatów
			if (!result.Succeeded &&
				result.Errors.Any(e => e.Code is "DuplicateUserName" or "DuplicateEmail"))
				return Conflict("Użytkownik z takim e‑mailem już istnieje.");

			if (!result.Succeeded)
				return BadRequest(result.Errors.Select(e => e.Description));

			return Ok();
		}

		// LOGIN
		/// <summary>
		/// Zwraca parę tokenó {access, refresh} lub 401 gdy dane błędne.
		/// </summary>
		[HttpPost("login")]
		public async Task<IActionResult> Login(LoginRequest request)
		{
			var query = _mapper.Map<LoginUserQuery>(request);
			var tokens = await _login.Handle(query);

			if (tokens is null) return Unauthorized("Nieprawidłowy e‑mail lub hasło.");

			return Ok(new AuthResponse
			{
				AccessToken = tokens.Value.access,
				RefreshToken = tokens.Value.refresh,
				ExpiresAt = DateTime.UtcNow.AddHours(3)
			});
		}

		// REFRESH
		/// <summary>
		/// Przyjmuje tokeny {access, refresh} i zwraca nowy zestaw tokenów.
		/// </summary>
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

		// LOGOUT
		/// <summary>
		/// Unieważnia podany refresh‑token i czyści cookies / identity.
		/// </summary>
		[Authorize]
		[HttpPost("logout")]
		public async Task<IActionResult> Logout(LogoutRequest req)
		{
			await _logout.Handle(new LogoutCommand(req.RefreshToken));
			await _signIn.SignOutAsync(); // czyści ewentualne cookies
			return Ok();
		}

		// TEST
		/// <summary>
		/// Szybki endpoint do sprawdzenia czy autoryzacja działa.
		/// </summary>
		[Authorize]
		[HttpGet("test")]
		public IActionResult TestAuth()
		{
			var email = User.FindFirstValue(ClaimTypes.Email);
			return Ok(new { message = "Autoryzacja przebiegła pomyślnie!", email });
		}
	}
}
