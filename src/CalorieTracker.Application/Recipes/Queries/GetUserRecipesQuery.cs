// Plik GetUserRecipesQuery.cs - definicja zapytania o przepisy użytkownika.
// Reprezentuje żądanie pobrania paginowanej listy przepisów należących do konkretnego użytkownika.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalorieTracker.Application.Recipes.Queries
{
	/// <summary>
	/// Klasa reprezentująca zapytanie o przepisy konkretnego użytkownika.
	/// Zawiera identyfikator użytkownika oraz parametry paginacji wyników.
	/// </summary>
	public class GetUserRecipesQuery
	{
		/// <summary>
		/// Identyfikator użytkownika, którego przepisy mają zostać zwrócone.
		/// Wymagany parametr bez domyślnej wartości.
		/// </summary>
		public string UserId { get; init; } = null!;

		/// <summary>
		/// Liczba przepisów do pominięcia (stronicowanie).
		/// Domyślnie 0 - zaczyna od początku listy.
		/// </summary>
		public int Skip { get; init; } = 0;

		/// <summary>
		/// Maksymalna liczba przepisów do zwrócenia.
		/// Domyślnie 20 - rozsądny kompromis między wydajnością a użytecznością.
		/// </summary>
		public int Take { get; init; } = 20;
	}
}