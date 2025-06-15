// Plik GetWeightMeasurementDetailsQuery.cs - definicja zapytania o szczegóły pomiaru wagi
// Odpowiada za przekazywanie żądania pobrania konkretnego pomiaru wagi użytkownika

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalorieTracker.Application.WeightMeasurements.Queries
{
	/// <summary>
	/// Rekord reprezentujący zapytanie o szczegóły konkretnego pomiaru wagi
	/// Zawiera identyfikator pomiaru oraz identyfikator użytkownika w celu weryfikacji dostępu
	/// </summary>
	public record GetWeightMeasurementDetailsQuery(Guid Id, string UserId);
}