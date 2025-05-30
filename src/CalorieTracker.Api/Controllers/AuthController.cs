using AutoMapper;
using CalorieTracker.Api.Attributes;
using CalorieTracker.Api.Models.Auth;
using CalorieTracker.Application.Auth.Commands;
using CalorieTracker.Application.Auth.Handlers;
using CalorieTracker.Application.Auth.Interfaces;
using CalorieTracker.Application.Auth.Queries;
using CalorieTracker.Application.Interfaces;
using CalorieTracker.Domain.Entities;
using CalorieTracker.Infrastructure.Data;
using CalorieTracker.Infrastructure.Email;                   
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
		//private readonly IEmailSender _mail;             
		//private readonly EmailSettings _mailCfg;          
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
			//_mail = mail;                            
			//_mailCfg = mailCfg.Value;                   
			_mapper = mapper;
		}

		// ---------- REGISTER ----------
		[HttpPost("register")]
		public async Task<IActionResult> Register(
			[FromBody] RegisterRequest request,
			[FromServices] UserManager<ApplicationUser> userManager,
			[FromServices] GenerateEmailConfirmationHandler confirmationHandler,
			[FromServices] AppDbContext db)
		{
			var cmd = _mapper.Map<RegisterUserCommand>(request);
			var result = await _register.Handle(cmd);

			if (!result.Succeeded &&
				result.Errors.Any(e => e.Code is "DuplicateUserName" or "DuplicateEmail"))
				return Conflict("Użytkownik z takim e-mailem już istnieje.");

			if (!result.Succeeded)
				return BadRequest(result.Errors.Select(e => e.Description));

			// Znajduje użytkownika i wysyła kod aktywacyjny
			var user = await userManager.FindByEmailAsync(request.Email);

			if (user is not null)
			{
				await confirmationHandler.Handle(user);

				// Dodaje pusty profil do bazy, jeśli nie istnieje
				var exists = await db.UserProfiles.AnyAsync(p => p.UserId == user.Id);
				if (!exists)
				{
					await db.UserProfiles.AddAsync(new UserProfile
					{
						UserId = user.Id
					});
					await db.SaveChangesAsync();
				}
			}

			return Ok();
		}

		// ---------- LOGIN ----------
		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequest request)
		{
			var user = await _signIn.UserManager.FindByEmailAsync(request.Email);
			if (user is null)
				return Unauthorized("Nieprawidłowy e‑mail lub hasło.");

			var passwordValid = await _signIn.UserManager.CheckPasswordAsync(user, request.Password);
			if (!passwordValid)
				return Unauthorized("Nieprawidłowy e‑mail lub hasło.");

			if (!await _signIn.UserManager.IsEmailConfirmedAsync(user))
				return StatusCode(403, "E-mail nie został potwierdzony.");

			// dane OK + email potwierdzony
			var query = _mapper.Map<LoginUserQuery>(request);
			var tokens = await _login.Handle(query);

			return Ok(new AuthResponse
			{
				AccessToken = tokens.Value.access,
				RefreshToken = tokens.Value.refresh,
				ExpiresAt = DateTime.UtcNow.AddMinutes(1)
			});
		}

		// ---------- CONFIRM ----------
		/// <summary>Kończy proces rejestracji – aktywuje konto.</summary>
		[HttpPost("confirm")]
		public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest req, [FromServices] UserManager<ApplicationUser> userManager, [FromServices] IAppDbContext db)
		{
			var user = await userManager.FindByEmailAsync(req.Email);
			if (user is null)
				return NotFound("Użytkownik nie istnieje");

			if (user.EmailConfirmed)
				return BadRequest("E-mail został już potwierdzony");

			var confirmation = await db.EmailConfirmations
				.FirstOrDefaultAsync(c => c.UserId == user.Id && c.Code == req.Code);

			if (confirmation is null)
				return BadRequest("Nieprawidłowy kod");

			if (confirmation.ExpiresAt < DateTime.UtcNow)
				return BadRequest("Kod wygasł");

			user.EmailConfirmed = true;
			await userManager.UpdateAsync(user);

			db.EmailConfirmations.Remove(confirmation);
			await db.SaveChangesAsync();

			return Ok("E-mail został potwierdzony");
		}

		// ---------- RESEND ----------
		/// <summary>Wysyła ponownie kod aktywacji na adres e-mail.</summary>
		[HttpPost("resend-code")]
		public async Task<IActionResult> ResendConfirmationCode([FromBody] ResendConfirmationRequest req, [FromServices] UserManager<ApplicationUser> userManager, [FromServices] GenerateEmailConfirmationHandler confirmationHandler, [FromServices] IAppDbContext db)
		{
			var user = await userManager.FindByEmailAsync(req.Email);
			if (user is null)
				return NotFound("Nie znaleziono użytkownika");

			if (user.EmailConfirmed)
				return BadRequest("Konto zostało już aktywowane.");

			var existing = await db.EmailConfirmations
				.Where(x => x.UserId == user.Id)
				.OrderByDescending(x => x.ExpiresAt)
				.FirstOrDefaultAsync();

			if (existing is not null)
			{
				var secondsAgo = (DateTime.UtcNow - existing.ExpiresAt.AddMinutes(-10)).TotalSeconds;
				if (secondsAgo < 60)
					return BadRequest($"Poczekaj {Math.Ceiling(60 - secondsAgo)} sekund przed ponownym wysłaniem.");
			}

			await confirmationHandler.Handle(user);

			return Ok("Nowy kod został wysłany.");
		}


		// ---------- REFRESH ----------
		[HttpPost("refresh")]
		public async Task<IActionResult> Refresh(RefreshRequest req)
		{
			var (access, refresh) = await _useRefresh.Handle(
										new UseRefreshTokenCommand(req.AccessToken, req.RefreshToken));

			if (access is null)
				return Unauthorized("Nieprawidłowe dane autoryzacji.");

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

		// ---------- FORGOT PASSWORD ----------
		[HttpPost("forgot-password")]
		public async Task<IActionResult> ForgotPassword(
			[FromBody] ForgotPasswordRequest request,
			[FromServices] UserManager<ApplicationUser> userManager,
			[FromServices] GeneratePasswordResetHandler handler)
		{
			var user = await userManager.FindByEmailAsync(request.Email);
			if (user is null || !await userManager.IsEmailConfirmedAsync(user))
				return Ok(); // nie zdradzamy nic

			await handler.Handle(user);
			return Ok("Kod resetujący hasło został wysłany.");
		}

		// ---------- RESET PASSWORD ----------
		[HttpPost("reset-password")]
		public async Task<IActionResult> ResetPassword(
			[FromBody] ResetPasswordRequest req,
			[FromServices] UserManager<ApplicationUser> userManager,
			[FromServices] IAppDbContext db)
		{
			var user = await userManager.FindByEmailAsync(req.Email);
			if (user is null)
				return BadRequest("Nieprawidłowy użytkownik.");

			var reset = await db.PasswordResets
				.FirstOrDefaultAsync(r => r.UserId == user.Id && r.Code == req.Code);

			if (reset is null)
				return BadRequest("Nieprawidłowy kod resetujący.");

			if (reset.ExpiresAt < DateTime.UtcNow)
				return BadRequest("Kod wygasł.");

			var result = await userManager.ResetPasswordAsync(user,
				await userManager.GeneratePasswordResetTokenAsync(user),
				req.NewPassword);

			if (!result.Succeeded)
				return BadRequest(result.Errors.Select(e => e.Description));

			db.PasswordResets.Remove(reset);
			await db.SaveChangesAsync();

			return Ok("Hasło zostało zresetowane.");
		}


		// ---------- TEST ----------
		[Authorize]
		[RequireCompleteProfile]
		[HttpGet("test")]
		public IActionResult TestAuth()
		{
			var email = User.FindFirstValue(ClaimTypes.Email);
			return Ok(new { message = "Autoryzacja przebiegła pomyślnie!", email });
		}
	}
}
