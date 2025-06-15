// Plik CreateRecipeIngredientCommand.cs - definicja komendy tworzenia składnika przepisu
// Odpowiada za przenoszenie danych wymaganych do utworzenia składnika w przepisie kulinarnym

namespace CalorieTracker.Application.Recipes.Commands
{
	/// <summary>
	/// Rekord reprezentujący komendę tworzenia składnika w przepisie kulinarnym.
	/// Zawiera informacje o produkcie i jego ilości potrzebnej w przepisie.
	/// </summary>
	/// <remarks>
	/// Wykorzystanie typu record zapewnia niezmienność danych transferowych.
	/// Stanowi część składową komendy <see cref="CreateRecipeCommand"/>.
	/// </remarks>
	public record CreateRecipeIngredientCommand
	{
		/// <summary>
		/// Identyfikator produktu użytego jako składnik
		/// </summary>
		/// <value>
		/// Wartość typu Guid reprezentująca unikalny identyfikator istniejącego produktu w systemie.
		/// </value>
		public Guid ProductId { get; init; }

		/// <summary>
		/// Ilość produktu w składniku przepisu
		/// </summary>
		/// <value>
		/// Wartość zmiennoprzecinkowa określająca ilość produktu potrzebną w przepisie.
		/// Wartość musi być dodatnia.
		/// </value>
		public float Quantity { get; init; }
	}
}