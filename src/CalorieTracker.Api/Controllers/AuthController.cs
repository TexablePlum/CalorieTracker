// Plik AuthController.cs - główny kontroler autoryzacji i uwierzytelniania aplikacji CalorieTracker.
// Odpowiada za zarządzanie rejestracją, logowaniem, potwierdzaniem kont i resetowaniem haseł użytkowników.

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
	/// <summary>
	/// Kontroler autoryzacji odpowiedzialny za zarządzanie procesami uwierzytelniania użytkowników.
	/// Obsługuje rejestrację, logowanie, potwierdzanie kont, odświeżanie tokenów oraz resetowanie haseł.
	/// Implementuje wzorzec CQRS wykorzystując dedykowane handlery dla każdej operacji.
	/// </summary>
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		/// <summary>
		/// Handler odpowiedzialny za proces rejestracji nowych użytkowników.
		/// </summary>
		private readonly RegisterUserHandler _register;

		/// <summary>
		/// Handler zarządzający procesem logowania użytkowników.
		/// </summary>
		private readonly LoginUserHandler _login;

		/// <summary>
		/// Handler obsługujący odświeżanie tokenów JWT.
		/// </summary>
		private readonly UseRefreshTokenHandler _useRefresh;

		/// <summary>
		/// Handler odpowiedzialny za proces wylogowywania użytkowników.
		/// </summary>
		private readonly LogoutHandler _logout;

		/// <summary>
		/// Manager odpowiedzialny za operacje logowania w systemie Identity.
		/// </summary>
		private readonly SignInManager<ApplicationUser> _signIn;

		/// <summary>
		/// Mapper AutoMapper do konwersji między modelami API a komendami/zapytaniami.
		/// </summary>
		private readonly IMapper _mapper;

		/// <summary>
		/// Inicjalizuje nową instancję kontrolera autoryzacji z wymaganymi zależnościami.
		/// </summary>
		/// <param name="register">Handler rejestracji użytkowników.</param>
		/// <param name="login">Handler logowania użytkowników.</param>
		/// <param name="useRefresh">Handler odświeżania tokenów.</param>
		/// <param name="logout">Handler wylogowywania.</param>
		/// <param name="signIn">Manager logowania Identity.</param>
		/// <param name="mail">Serwis wysyłki emaili.</param>
		/// <param name="mailCfg">Konfiguracja emaili.</param>
		/// <param name="mapper">Mapper AutoMapper.</param>
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
			_mapper = mapper;
		}

		/// <summary>
		/// Rejestruje nowego użytkownika w systemie.
		/// Tworzy konto użytkownika, wysyła kod weryfikacyjny email oraz inicjalizuje pusty profil użytkownika.
		/// </summary>
		/// <param name="request">Dane rejestracyjne użytkownika zawierające email, hasło i opcjonalne imię/nazwisko.</param>
		/// <param name="userManager">Manager użytkowników Identity (wstrzykiwany przez DI).</param>
		/// <param name="confirmationHandler">Handler generowania kodów potwierdzających email (wstrzykiwany przez DI).</param>
		/// <param name="db">Kontekst bazy danych (wstrzykiwany przez DI).</param>
		/// <returns>
		/// 200 OK - gdy rejestracja przebiegła pomyślnie i kod weryfikacyjny został wysłany.
		/// 409 Conflict - gdy użytkownik z podanym emailem już istnieje.
		/// 400 BadRequest - gdy wystąpiły błędy walidacji hasła lub innych danych.
		/// </returns>
		[HttpPost("register")]
		public async Task<IActionResult> Register(
			   [FromBody] RegisterRequest request,
			   [FromServices] UserManager<ApplicationUser> userManager,
			   [FromServices] GenerateEmailConfirmationHandler confirmationHandler,
			   [FromServices] AppDbContext db)
		{
			// Mapuje żądanie na komendę i wykonuje proces rejestracji
			var cmd = _mapper.Map<RegisterUserCommand>(request);
			var result = await _register.Handle(cmd);

			// Obsługuje błędy duplikacji użytkownika
			if (!result.Succeeded &&
				   result.Errors.Any(e => e.Code is "DuplicateUserName" or "DuplicateEmail"))
				return Conflict("Użytkownik z takim e-mailem już istnieje.");

			// Obsługuje inne błędy rejestracji
			if (!result.Succeeded)
				return BadRequest(result.Errors.Select(e => e.Description));

			// Wysyła kod aktywacyjny do nowo zarejestrowanego użytkownika
			var user = await userManager.FindByEmailAsync(request.Email);

			if (user is not null)
			{
				await confirmationHandler.Handle(user);

				// Tworzy pusty profil użytkownika w bazie danych
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

		/// <summary>
		/// Loguje użytkownika do systemu.
		/// Weryfikuje dane logowania, sprawdza potwierdzenie emaila i generuje tokeny JWT.
		/// </summary>
		/// <param name="request">Dane logowania zawierające email i hasło użytkownika.</param>
		/// <returns>
		/// 200 OK z tokenami JWT - gdy logowanie przebiegło pomyślnie.
		/// 401 Unauthorized - gdy email lub hasło są nieprawidłowe.
		/// 403 Forbidden - gdy email nie został potwierdzony.
		/// </returns>
		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequest request)
		{
			// Weryfikuje istnienie użytkownika
			var user = await _signIn.UserManager.FindByEmailAsync(request.Email);
			if (user is null)
				return Unauthorized("Nieprawidłowy e‑mail lub hasło.");

			// Sprawdza poprawność hasła
			var passwordValid = await _signIn.UserManager.CheckPasswordAsync(user, request.Password);
			if (!passwordValid)
				return Unauthorized("Nieprawidłowy e‑mail lub hasło.");

			// Weryfikuje potwierdzenie adresu email
			if (!await _signIn.UserManager.IsEmailConfirmedAsync(user))
				return StatusCode(403, "E-mail nie został potwierdzony.");

			// Generuje tokeny JWT po pomyślnej weryfikacji
			var query = _mapper.Map<LoginUserQuery>(request);
			var tokens = await _login.Handle(query);

			return Ok(new AuthResponse
			{
				AccessToken = tokens.Value.access,
				RefreshToken = tokens.Value.refresh,
				ExpiresAt = DateTime.UtcNow.AddMinutes(1)
			});
		}

		/// <summary>
		/// Potwierdza adres email użytkownika kodem weryfikacyjnym.
		/// Kończy proces rejestracji aktywując konto użytkownika.
		/// </summary>
		/// <param name="req">Żądanie zawierające email i 6-cyfrowy kod weryfikacyjny.</param>
		/// <param name="userManager">Manager użytkowników Identity (wstrzykiwany przez DI).</param>
		/// <param name="db">Kontekst bazy danych (wstrzykiwany przez DI).</param>
		/// <returns>
		/// 200 OK - gdy email został pomyślnie potwierdzony.
		/// 404 NotFound - gdy użytkownik o podanym emailu nie istnieje.
		/// 400 BadRequest - gdy email już został potwierdzony, kod jest nieprawidłowy lub wygasł.
		/// </returns>
		[HttpPost("confirm")]
		public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest req, [FromServices] UserManager<ApplicationUser> userManager, [FromServices] IAppDbContext db)
		{
			// Sprawdza istnienie użytkownika
			var user = await userManager.FindByEmailAsync(req.Email);
			if (user is null)
				return NotFound("Użytkownik nie istnieje");

			// Weryfikuje czy email nie został już potwierdzony
			if (user.EmailConfirmed)
				return BadRequest("E-mail został już potwierdzony");

			// Wyszukuje rekord potwierdzenia z podanym kodem
			var confirmation = await db.EmailConfirmations
				   .FirstOrDefaultAsync(c => c.UserId == user.Id && c.Code == req.Code);

			if (confirmation is null)
				return BadRequest("Nieprawidłowy kod");

			// Sprawdza czy kod nie wygasł
			if (confirmation.ExpiresAt < DateTime.UtcNow)
				return BadRequest("Kod wygasł");

			// Aktywuje konto użytkownika
			user.EmailConfirmed = true;
			await userManager.UpdateAsync(user);

			// Usuwa wykorzystany kod potwierdzenia
			db.EmailConfirmations.Remove(confirmation);
			await db.SaveChangesAsync();

			return Ok("E-mail został potwierdzony");
		}

		/// <summary>
		/// Wysyła ponownie kod aktywacyjny na adres email użytkownika.
		/// Implementuje throttling aby zapobiec spamowaniu.
		/// </summary>
		/// <param name="req">Żądanie zawierające adres email użytkownika.</param>
		/// <param name="userManager">Manager użytkowników Identity (wstrzykiwany przez DI).</param>
		/// <param name="confirmationHandler">Handler generowania kodów potwierdzających (wstrzykiwany przez DI).</param>
		/// <param name="db">Kontekst bazy danych (wstrzykiwany przez DI).</param>
		/// <returns>
		/// 200 OK - gdy nowy kod został wysłany.
		/// 404 NotFound - gdy użytkownik nie istnieje.
		/// 400 BadRequest - gdy konto już zostało aktywowane lub nie minął czas throttlingu (60 sekund).
		/// </returns>
		[HttpPost("resend-code")]
		public async Task<IActionResult> ResendConfirmationCode([FromBody] ResendConfirmationRequest req, [FromServices] UserManager<ApplicationUser> userManager, [FromServices] GenerateEmailConfirmationHandler confirmationHandler, [FromServices] IAppDbContext db)
		{
			// Weryfikuje istnienie użytkownika
			var user = await userManager.FindByEmailAsync(req.Email);
			if (user is null)
				return NotFound("Nie znaleziono użytkownika");

			// Sprawdza czy konto nie zostało już aktywowane
			if (user.EmailConfirmed)
				return BadRequest("Konto zostało już aktywowane.");

			// Implementuje throttling - sprawdza czy można wysłać nowy kod
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

			// Generuje i wysyła nowy kod potwierdzający
			await confirmationHandler.Handle(user);

			return Ok("Nowy kod został wysłany.");
		}

		/// <summary>
		/// Odświeża tokeny JWT przy użyciu refresh tokenu.
		/// Implementuje rotację tokenów dla zwiększenia bezpieczeństwa.
		/// </summary>
		/// <param name="req">Żądanie zawierające aktualny access token i refresh token.</param>
		/// <returns>
		/// 200 OK z nowymi tokenami - gdy odświeżenie przebiegło pomyślnie.
		/// 401 Unauthorized - gdy podane tokeny są nieprawidłowe lub wygasłe.
		/// </returns>
		[HttpPost("refresh")]
		public async Task<IActionResult> Refresh(RefreshRequest req)
		{
			// Wykonuje operację odświeżenia tokenów
			var (access, refresh) = await _useRefresh.Handle(
									   new UseRefreshTokenCommand(req.AccessToken, req.RefreshToken));

			// Sprawdza czy operacja zakończyła się sukcesem
			if (access is null)
				return Unauthorized("Nieprawidłowe dane autoryzacji.");

			return Ok(new
			{
				accessToken = access,
				refreshToken = refresh,
				expiresAt = DateTime.UtcNow.AddHours(3)
			});
		}

		/// <summary>
		/// Wylogowuje użytkownika z systemu.
		/// Unieważnia refresh token i kończy sesję w systemie Identity.
		/// </summary>
		/// <param name="req">Żądanie zawierające refresh token do unieważnienia.</param>
		/// <returns>200 OK po pomyślnym wylogowaniu.</returns>
		[Authorize]
		[HttpPost("logout")]
		public async Task<IActionResult> Logout(LogoutRequest req)
		{
			// Unieważnia refresh token
			await _logout.Handle(new LogoutCommand(req.RefreshToken));

			// Kończy sesję w systemie Identity
			await _signIn.SignOutAsync();

			return Ok();
		}

		/// <summary>
		/// Inicjuje proces resetowania hasła użytkownika.
		/// Generuje i wysyła kod resetujący na adres email użytkownika.
		/// </summary>
		/// <param name="request">Żądanie zawierające adres email użytkownika.</param>
		/// <param name="userManager">Manager użytkowników Identity (wstrzykiwany przez DI).</param>
		/// <param name="handler">Handler generowania kodów resetujących hasło (wstrzykiwany przez DI).</param>
		/// <returns>
		/// 200 OK - zawsze (ze względów bezpieczeństwa nie ujawnia czy użytkownik istnieje).
		/// </returns>
		[HttpPost("forgot-password")]
		public async Task<IActionResult> ForgotPassword(
			   [FromBody] ForgotPasswordRequest request,
			   [FromServices] UserManager<ApplicationUser> userManager,
			   [FromServices] GeneratePasswordResetHandler handler)
		{
			// Sprawdza czy użytkownik istnieje i ma potwierdzony email
			var user = await userManager.FindByEmailAsync(request.Email);
			if (user is null || !await userManager.IsEmailConfirmedAsync(user))
				return Ok(); // Nie ujawnia informacji o istnieniu użytkownika

			// Generuje i wysyła kod resetujący hasło
			await handler.Handle(user);
			return Ok("Kod resetujący hasło został wysłany.");
		}

		/// <summary>
		/// Resetuje hasło użytkownika przy użyciu kodu weryfikacyjnego.
		/// Kończy proces resetowania hasła ustawiając nowe hasło.
		/// </summary>
		/// <param name="req">Żądanie zawierające email, kod resetujący i nowe hasło.</param>
		/// <param name="userManager">Manager użytkowników Identity (wstrzykiwany przez DI).</param>
		/// <param name="db">Kontekst bazy danych (wstrzykiwany przez DI).</param>
		/// <returns>
		/// 200 OK - gdy hasło zostało pomyślnie zresetowane.
		/// 400 BadRequest - gdy użytkownik nie istnieje, kod jest nieprawidłowy/wygasły lub nowe hasło nie spełnia wymagań.
		/// </returns>
		[HttpPost("reset-password")]
		public async Task<IActionResult> ResetPassword(
			   [FromBody] ResetPasswordRequest req,
			   [FromServices] UserManager<ApplicationUser> userManager,
			   [FromServices] IAppDbContext db)
		{
			// Weryfikuje istnienie użytkownika
			var user = await userManager.FindByEmailAsync(req.Email);
			if (user is null)
				return BadRequest("Nieprawidłowy użytkownik.");

			// Sprawdza poprawność kodu resetującego
			var reset = await db.PasswordResets
				   .FirstOrDefaultAsync(r => r.UserId == user.Id && r.Code == req.Code);

			if (reset is null)
				return BadRequest("Nieprawidłowy kod resetujący.");

			// Weryfikuje czy kod nie wygasł
			if (reset.ExpiresAt < DateTime.UtcNow)
				return BadRequest("Kod wygasł.");

			// Resetuje hasło użytkownika
			var result = await userManager.ResetPasswordAsync(user,
				   await userManager.GeneratePasswordResetTokenAsync(user),
				   req.NewPassword);

			if (!result.Succeeded)
				return BadRequest(result.Errors.Select(e => e.Description));

			// Usuwa wykorzystany kod resetujący
			db.PasswordResets.Remove(reset);
			await db.SaveChangesAsync();

			return Ok("Hasło zostało zresetowane.");
		}

		/// <summary>
		/// Pobiera podstawowe informacje o aktualnie zalogowanym użytkowniku.
		/// Zwraca ID i adres email użytkownika na podstawie claims z tokenu JWT.
		/// </summary>
		/// <returns>
		/// 200 OK z danymi użytkownika - gdy użytkownik jest zalogowany.
		/// 401 Unauthorized - gdy brak autoryzacji.
		/// </returns>
		[Authorize]
		[HttpGet("me")]
		public IActionResult GetCurrentUser()
		{
			// Ekstraktuje dane użytkownika z claims tokenu JWT
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var email = User.FindFirstValue(ClaimTypes.Email);

			return Ok(new
			{
				id = userId,
				email = email
			});
		}

		/// <summary>
		/// Endpoint testowy do weryfikacji poprawności autoryzacji JWT.
		/// Służy do debugowania i testowania mechanizmów uwierzytelniania.
		/// </summary>
		/// <returns>
		/// 200 OK z komunikatem potwierdzającym - gdy autoryzacja jest poprawna.
		/// 401 Unauthorized - gdy brak autoryzacji lub token jest nieprawidłowy.
		/// </returns>
		[Authorize]
		[HttpGet("test")]
		public IActionResult TestAuth()
		{
			var email = User.FindFirstValue(ClaimTypes.Email);
			return Ok(new { message = "Autoryzacja przebiegła pomyślnie!", email });
		}
	}
}