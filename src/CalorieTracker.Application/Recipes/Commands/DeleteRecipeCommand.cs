// Plik DeleteRecipeCommand.cs - definicja komendy usuwania przepisu
// Odpowiada za przenoszenie danych wymaganych do usunięcia przepisu z systemu

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalorieTracker.Application.Recipes.Commands
{
	/// <summary>
	/// Rekord reprezentujący komendę usuwania przepisu kulinarnego.
	/// Zawiera identyfikator przepisu oraz informację o użytkowniku wykonującym operację.
	/// </summary>
	/// <remarks>
	/// Wykorzystanie typu record gwarantuje niezmienność danych transferowych.
	/// Komenda powinna być poprzedzona weryfikacją uprawnień użytkownika.
	/// </remarks>
	public record DeleteRecipeCommand(
		/// <summary>
		/// Identyfikator przepisu do usunięcia
		/// </summary>
		/// <value>
		/// Wartość typu Guid reprezentująca unikalny identyfikator istniejącego przepisu w systemie.
		/// </value>
		Guid Id,

		/// <summary>
		/// Identyfikator użytkownika wykonującego operację usunięcia
		/// </summary>
		/// <value>
		/// Ciąg znaków reprezentujący identyfikator użytkownika.
		/// Wymagany do weryfikacji uprawnień do wykonania operacji.
		/// </value>
		string DeletedByUserId
	);
}