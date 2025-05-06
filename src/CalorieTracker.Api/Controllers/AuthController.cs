using AutoMapper;
using CalorieTracker.Api.Models.Auth;
using CalorieTracker.Application.Auth.Commands;
using CalorieTracker.Application.Auth.Handlers;
using CalorieTracker.Application.Auth.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CalorieTracker.Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly RegisterUserHandler _registerHandler;
		private readonly LoginUserHandler _loginHandler;
		private readonly IMapper _mapper;

		public AuthController(
			RegisterUserHandler registerHandler,
			LoginUserHandler loginHandler,
			IMapper mapper)
		{
			_registerHandler = registerHandler;
			_loginHandler = loginHandler;
			_mapper = mapper;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register(RegisterRequest request)
		{
			var cmd = _mapper.Map<RegisterUserCommand>(request);
			var result = await _registerHandler.Handle(cmd);
			if (!result.Succeeded)
				return BadRequest(result.Errors);

			return Ok();
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login(LoginRequest request)
		{
			var query = _mapper.Map<LoginUserQuery>(request);
			var token = await _loginHandler.Handle(query);
			if (token is null)
				return Unauthorized();

			return Ok(new AuthResponse
			{
				Token = token,
				ExpiresAt = DateTime.UtcNow.AddHours(3)
			});
		}

		[Authorize]
		[HttpGet("test")]
		public IActionResult TestAuth()
		{
			var userEmail = User.FindFirstValue(ClaimTypes.Email);

			return Ok(new
			{
				message = "Token działa i chroniony endpoint zwrócony!",
				email = userEmail
			});
		}
	}
}
