// Plik JwtGenerator.cs - implementacja generatora tokenów JWT.
// Odpowiada za tworzenie tokenów JWT używanych do uwierzytelniania i autoryzacji użytkowników w aplikacji.

using CalorieTracker.Application.Auth.Interfaces;
using CalorieTracker.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CalorieTracker.Infrastructure.Auth
{
	/// &lt;summary>
	/// Implementacja interfejsu &lt;see cref="IJwtGenerator"/> odpowiedzialna za generowanie tokenów JWT.
	/// Wykorzystuje konfigurację aplikacji do pobierania klucza, wystawcy i odbiorcy tokena.
	/// &lt;/summary>
	public class JwtGenerator : IJwtGenerator
	{
		/// &lt;summary>
		/// Prywatne pole tylko do odczytu, przechowujące konfigurację aplikacji.
		/// &lt;/summary>
		private readonly IConfiguration _config;

		/// <summary>
		/// Inicjalizuje nową instancję klasy <see cref="JwtGenerator"/>.
		/// </summary>
		/// <param name="config">Obiekt konfiguracji aplikacji <see cref="IConfiguration"/>.</param>
		public JwtGenerator(IConfiguration config) => _config = config;

		/// <summary>
		/// Tworzy token JWT dla podanego użytkownika.
		/// Token zawiera identyfikator użytkownika oraz jego adres e-mail jako roszczenia (claims).
		/// </summary>
		/// <param name="user">Obiekt <see cref="ApplicationUser"/>, dla którego token ma zostać wygenerowany.</param>
		/// <returns>Ciąg znaków reprezentujący wygenerowany token JWT.</returns>
		public string CreateToken(ApplicationUser user)
		{
			// Definicja roszczeń (claims) zawartych w tokenie.
			var claims = new[]
			{
			new Claim(JwtRegisteredClaimNames.Sub, user.Id), // Identyfikator podmiotu (użytkownika)
			new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty) // Adres e-mail użytkownika
		};

			// Pobranie klucza szyfrującego z konfiguracji i utworzenie symetrycznego klucza bezpieczeństwa.
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
			// Utworzenie poświadczeń podpisywania, używając klucza i algorytmu HMAC SHA256.
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			// Utworzenie obiektu tokena JWT.
			var token = new JwtSecurityToken(
				issuer: _config["Jwt:Issuer"],			// Wystawca tokena
				audience: _config["Jwt:Audience"],		// Odbiorca tokena
				claims: claims,							// Roszczenia tokena
				expires: DateTime.UtcNow.AddHours(3),	// Data i czas wygaśnięcia tokena (3 godziny od teraz)
				signingCredentials: creds				// Poświadczenia podpisywania
			);

			// Serializacja tokena do postaci ciągu znaków i zwrócenie go.
			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}