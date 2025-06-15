// Plik LoginUserHandler.cs - implementacja handlera logowania użytkownika.
// Odpowiada za cały proces uwierzytelniania użytkownika, w tym walidację danych, generację tokenów JWT i refresh tokenów.

using CalorieTracker.Application.Auth.Commands;
using CalorieTracker.Application.Auth.Interfaces;
using CalorieTracker.Application.Auth.Queries;
using CalorieTracker.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CalorieTracker.Application.Auth.Handlers
{
	/// <summary>
	/// Handler odpowiedzialny za proces logowania użytkownika.
	/// Wykonuje pełną procedurę uwierzytelniania, w tym:
	/// - Weryfikację poprawności danych logowania
	/// - Sprawdzenie statusu potwierdzenia emaila
	/// - Generację tokena JWT
	/// - Generację refresh tokena
	/// - Zarządzanie blokadą konta po nieudanych próbach
	/// </summary>
	public class LoginUserHandler
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly IJwtGenerator _jwtGenerator;
		private readonly GenerateRefreshTokenHandler _generateRt;

		/// <summary>
		/// Inicjalizuje nową instancję handlera logowania.
		/// </summary>
		/// <param name="userManager">Menadżer użytkowników <see cref="UserManager{ApplicationUser}"/>.</param>
		/// <param name="signInManager">Menadżer logowania <see cref="SignInManager{ApplicationUser}"/>.</param>
		/// <param name="jwtGenerator">Generator tokenów JWT <see cref="IJwtGenerator"/>.</param>
		/// <param name="generateRt">Handler generowania refresh tokenów <see cref="GenerateRefreshTokenHandler"/>.</param>
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

		/// <summary>
		/// Główna metoda handlera wykonująca proces logowania użytkownika.
		/// </summary>
		/// <param name="query">Zapytanie <see cref="LoginUserQuery"/> zawierające dane logowania.</param>
		/// <returns>
		/// Krotkę zawierającą token dostępowy i refresh token, gdy logowanie powiedzie się;
		/// null, gdy: błędne dane, konto zablokowane lub niepotwierdzony email.
		/// </returns>
		public async Task<(string access, string refresh)?> Handle(LoginUserQuery query)
		{
			// Wyszukuje użytkownika po adresie email
			var user = await _userManager.FindByEmailAsync(query.Email);
			if (user is null) return null;

			// Wymaga potwierdzonego adresu email
			if (!await _userManager.IsEmailConfirmedAsync(user))
				return null;

			// Weryfikuje hasło i zarządza blokadą konta
			var signInRes = await _signInManager.PasswordSignInAsync(
								user, query.Password, false, true);
			if (!signInRes.Succeeded) return null;

			// Generuje token dostępowy JWT
			var access = _jwtGenerator.CreateToken(user);

			// Generuje nowy refresh token
			var refresh = await _generateRt.Handle(
							  new GenerateRefreshTokenCommand(user.Id));

			return (access, refresh);
		}
	}
}