// Plik GetLatestWeightMeasurementQuery.cs - definicja zapytania o najnowszy pomiar wagi.
// Odpowiada za przekazywanie żądania pobrania najnowszego pomiaru wagi dla danego użytkownika.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalorieTracker.Application.WeightMeasurements.Queries
{
	/// <summary>
	/// Rekord reprezentujący zapytanie o najnowszy pomiar wagi użytkownika.
	/// Przenosi identyfikator użytkownika, dla którego ma zostać pobrany pomiar.
	/// </summary>
	public record GetLatestWeightMeasurementQuery(string UserId);
}