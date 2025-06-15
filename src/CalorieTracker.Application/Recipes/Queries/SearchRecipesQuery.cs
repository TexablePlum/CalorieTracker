// Plik SearchRecipesQuery.cs - definicja zapytania o wyszukiwanie przepisów.
// Reprezentuje żądanie przeszukania przepisów z możliwością paginacji wyników.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalorieTracker.Application.Recipes.Queries
{
	/// <summary>
	/// Klasa reprezentująca zapytanie o wyszukiwanie przepisów.
	/// Umożliwia przeszukiwanie przepisów z użyciem frazy oraz obsługuje paginację wyników.
	/// </summary>
	public class SearchRecipesQuery
	{
		/// <summary>
		/// Fraza używana do wyszukiwania przepisów.
		/// Wymagany parametr bez domyślnej wartości.
		/// </summary>
		public string SearchTerm { get; init; } = null!;

		/// <summary>
		/// Liczba przepisów do pominięcia (stronicowanie).
		/// Domyślnie 0 - zaczyna od początku listy wyników.
		/// </summary>
		public int Skip { get; init; } = 0;

		/// <summary>
		/// Maksymalna liczba przepisów do zwrócenia.
		/// Domyślnie 20 - zapewnia równowagę między wydajnością a użytecznością.
		/// </summary>
		public int Take { get; init; } = 20;
	}
}