// Plik CreateWeightMeasurementCommand.cs - definicja komendy tworzenia pomiaru wagi.
// Reprezentuje żądanie dodania nowego pomiaru masy ciała dla określonego użytkownika.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalorieTracker.Application.WeightMeasurements.Commands
{
	/// <summary>
	/// Klasa reprezentująca komendę tworzenia nowego pomiaru masy ciała.
	/// Zawiera wszystkie niezbędne dane wymagane do utworzenia pomiaru w systemie.
	/// </summary>
	public record CreateWeightMeasurementCommand
	{
		/// <summary>
		/// Identyfikator użytkownika, dla którego tworzony jest pomiar.
		/// Wymagany parametr bez domyślnej wartości.
		/// </summary>
		public string UserId { get; init; } = null!;

		/// <summary>
		/// Data wykonania pomiaru.
		/// Wymagany parametr bez domyślnej wartości.
		/// </summary>
		public DateOnly MeasurementDate { get; init; }

		/// <summary>
		/// Wartość pomiaru w kilogramach.
		/// Wymagany parametr bez domyślnej wartości.
		/// </summary>
		public float WeightKg { get; init; }
	}
}