// Plik IJwtGenerator.cs - definicja interfejsu generatora tokenów JWT.
// Określa kontrakt dla komponentu odpowiedzialnego za generowanie tokenów uwierzytelniających.

using CalorieTracker.Domain.Entities;

namespace CalorieTracker.Application.Auth.Interfaces
{
	/// <summary>
	/// Interfejs generatora tokenów JWT (JSON Web Tokens).
	/// Definiuje operację tworzenia tokenów uwierzytelniających dla użytkowników systemu.
	/// </summary>
	public interface IJwtGenerator
	{
		/// <summary>
		/// Generuje token JWT dla określonego użytkownika.
		/// </summary>
		/// <param name="user">Obiekt użytkownika <see cref="ApplicationUser"/>, dla którego generowany jest token.
		/// Powinien zawierać minimalnie identyfikator i adres email użytkownika.</param>
		/// <returns>
		/// Ciąg znaków reprezentujący wygenerowany token JWT w formacie zgodnym z RFC 7519.
		/// Token zawiera standardowe claimy (sub, email) oraz informacje o ważności.
		/// </returns>
		string CreateToken(ApplicationUser user);
	}
}