// Plik UpdateWeightMeasurementCommand.cs - definicja komendy aktualizacji pomiaru wagi.
// Reprezentuje żądanie zmiany istniejącego pomiaru masy ciała użytkownika.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalorieTracker.Application.WeightMeasurements.Commands
{
	/// <summary>
	/// Klasa reprezentująca komendę aktualizacji istniejącego pomiaru masy ciała.
	/// Zawiera wszystkie wymagane dane do identyfikacji i modyfikacji pomiaru w systemie.
	/// </summary>
	public record UpdateWeightMeasurementCommand
	{
		/// <summary>
		/// Identyfikator pomiaru do zaktualizowania.
		/// Wymagany parametr bez domyślnej wartości.
		/// </summary>
		public Guid Id { get; init; }

		/// <summary>
		/// Identyfikator użytkownika wykonującego operację.
		/// Wymagany parametr bez domyślnej wartości.
		/// </summary>
		public string UserId { get; init; } = null!;

		/// <summary>
		/// Nowa data pomiaru.
		/// Wymagany parametr bez domyślnej wartości.
		/// </summary>
		public DateOnly MeasurementDate { get; init; }

		/// <summary>
		/// Nowa wartość pomiaru w kilogramach.
		/// Wymagany parametr bez domyślnej wartości.
		/// </summary>
		public float WeightKg { get; init; }
	}
}