// Plik RegisterUserHandler.cs - implementacja handlera rejestracji nowego użytkownika.
// Odpowiada za proces tworzenia nowego konta użytkownika w systemie.

using CalorieTracker.Application.Auth.Commands;
using CalorieTracker.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CalorieTracker.Application.Auth.Handlers
{
	/// <summary>
	/// Handler odpowiedzialny za proces rejestracji nowego użytkownika.
	/// Tworzy nowe konto użytkownika z podanymi danymi i hasłem.
	/// </summary>
	public class RegisterUserHandler
	{
		/// <summary>
		/// Menadżer użytkowników odpowiedzialny za operacje na kontach.
		/// </summary>
		private readonly UserManager<ApplicationUser> _userManager;

		/// <summary>
		/// Inicjalizuje nową instancję handlera rejestracji.
		/// </summary>
		/// <param name="userManager">Menadżer użytkowników <see cref="UserManager{ApplicationUser}"/>.</param>
		public RegisterUserHandler(UserManager<ApplicationUser> userManager)
			=> _userManager = userManager;

		/// <summary>
		/// Główna metoda handlera wykonująca rejestrację nowego użytkownika.
		/// </summary>
		/// <param name="cmd">Komenda <see cref="RegisterUserCommand"/> zawierająca dane rejestracyjne.</param>
		/// <returns>
		/// Wynik operacji <see cref="IdentityResult"/> zawierający informacje o powodzeniu 
		/// lub błędach podczas tworzenia konta.
		/// </returns>
		public async Task<IdentityResult> Handle(RegisterUserCommand cmd)
		{
			// Tworzy nowego użytkownika na podstawie danych z komendy
			var user = new ApplicationUser
			{
				UserName = cmd.Email,    // Nazwa użytkownika ustawiana jako email
				Email = cmd.Email,      // Adres email użytkownika
				FirstName = cmd.FirstName, // Opcjonalne imię użytkownika
				LastName = cmd.LastName   // Opcjonalne nazwisko użytkownika
			};

			// Próbuje utworzyć użytkownika z podanym hasłem
			return await _userManager.CreateAsync(user, cmd.Password);
		}
	}
}