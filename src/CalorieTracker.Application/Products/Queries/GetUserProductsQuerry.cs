// Plik GetUserProductsQuery.cs - definicja zapytania o pobranie produktów użytkownika
// Odpowiada za przenoszenie danych potrzebnych do pobrania paginowanej listy produktów przypisanych do użytkownika

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalorieTracker.Application.Auth.Queries
{
	/// <summary>
	/// Klasa reprezentująca zapytanie o pobranie produktów powiązanych z określonym użytkownikiem.
	/// Zawiera parametry paginacji oraz identyfikator użytkownika.
	/// </summary>
	/// <remarks>
	/// Implementacja jako klasy (zamiast record) pozwala na domyślne wartości właściwości.
	/// Właściwości init-only zapewniają niezmienność obiektu po inicjalizacji.
	/// </remarks>
	public class GetUserProductsQuery
	{
		/// <summary>
		/// Identyfikator użytkownika, dla którego pobierane są produkty
		/// </summary>
		/// <value>
		/// Ciąg znaków reprezentujący unikalny identyfikator użytkownika w systemie.
		/// Wartość musi być różna od null.
		/// </value>
		public string UserId { get; init; } = null!;

		/// <summary>
		/// Liczba produktów do pominięcia (stronicowanie)
		/// </summary>
		/// <value>
		/// Domyślna wartość: 0. Pozwala na implementację mechanizmu paginacji.
		/// </value>
		public int Skip { get; init; } = 0;

		/// <summary>
		/// Maksymalna liczba produktów do pobrania (stronicowanie)
		/// </summary>
		/// <value>
		/// Domyślna wartość: 20. Określa rozmiar strony wyników.
		/// </value>
		public int Take { get; init; } = 20;
	}
}