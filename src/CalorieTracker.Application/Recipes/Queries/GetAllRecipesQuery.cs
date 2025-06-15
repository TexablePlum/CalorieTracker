// Plik GetAllRecipesQuery.cs - definicja zapytania o listę przepisów.
// Reprezentuje żądanie pobrania paginowanej listy wszystkich przepisów w systemie.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalorieTracker.Application.Recipes.Queries
{
	/// <summary>
	/// Klasa reprezentująca zapytanie o globalną listę przepisów.
	/// Zawiera parametry paginacji do kontroli ilości zwracanych wyników.
	/// </summary>
	public class GetAllRecipesQuery
	{
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