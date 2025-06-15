// Plik GetUserWeightMeasurementsQuery.cs - definicja zapytania o pomiary wagi z paginacją
// Odpowiada za przekazywanie żądania pobrania paginowanej listy pomiarów wagi użytkownika

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalorieTracker.Application.WeightMeasurements.Queries
{
	/// <summary>
	/// Klasa reprezentująca zapytanie o paginowaną listę pomiarów wagi użytkownika
	/// Przenosi parametry paginacji oraz identyfikator użytkownika
	/// </summary>
	public class GetUserWeightMeasurementsQuery
	{
		/// <summary>
		/// Identyfikator użytkownika, dla którego pobierane są pomiary
		/// Wymagany, nie może być null
		/// </summary>
		public string UserId { get; init; } = null!;

		/// <summary>
		/// Liczba pomiarów do pominięcia (stronicowanie)
		/// Domyślna wartość: 0
		/// </summary>
		public int Skip { get; init; } = 0;

		/// <summary>
		/// Maksymalna liczba pomiarów do zwrócenia
		/// Domyślna wartość: 20
		/// </summary>
		public int Take { get; init; } = 20;
	}
}