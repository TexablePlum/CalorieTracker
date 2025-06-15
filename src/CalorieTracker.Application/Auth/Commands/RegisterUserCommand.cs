// Plik RegisterUserCommand.cs - definicja komendy rejestracji nowego użytkownika.
// Odpowiada za przekazanie danych wymaganych do procesu rejestracji nowego konta użytkownika w systemie.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalorieTracker.Application.Auth.Commands
{
	/// <summary>
	/// Klasa reprezentująca komendę rejestracji nowego użytkownika.
	/// Zawiera wszystkie niezbędne dane wymagane do utworzenia nowego konta użytkownika.
	/// </summary>
	public class RegisterUserCommand
	{
		/// <summary>
		/// Adres e-mail użytkownika. Wymagany do rejestracji.
		/// </summary>
		public string Email { get; init; } = null!;

		/// <summary>
		/// Hasło użytkownika. Wymagane do rejestracji.
		/// </summary>
		public string Password { get; init; } = null!;

		/// <summary>
		/// Imię użytkownika. Opcjonalne pole.
		/// </summary>
		public string? FirstName { get; init; }

		/// <summary>
		/// Nazwisko użytkownika. Opcjonalne pole.
		/// </summary>
		public string? LastName { get; init; }
	}
}