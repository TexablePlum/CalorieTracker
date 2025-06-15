// Plik `ApplicationUser.cs` - definicja encji użytkownika aplikacji.
// Rozszerza domyślną klasę IdentityUser o dodatkowe, specyficzne dla aplikacji właściwości.

using Microsoft.AspNetCore.Identity;

namespace CalorieTracker.Domain.Entities
{
	/// <summary>
	/// Reprezentacja użytkownika w systemie.
	/// </summary>
	public class ApplicationUser : IdentityUser
	{
		/// <summary>
		/// Przechowywanie imienia użytkownika.
		/// </summary>
		public string? FirstName { get; set; }

		/// <summary>
		/// Przechowywanie nazwiska użytkownika.
		/// </summary>
		public string? LastName { get; set; }

		/// <summary>
		/// Właściwość nawigacyjna do szczegółowego profilu użytkownika.
		/// </summary>
		public UserProfile? Profile { get; set; }
	}
}