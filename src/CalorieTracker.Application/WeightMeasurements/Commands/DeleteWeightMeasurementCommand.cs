// Plik DeleteWeightMeasurementCommand.cs - definicja komendy usuwania pomiaru wagi.
// Reprezentuje żądanie usunięcia istniejącego pomiaru masy ciała użytkownika.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalorieTracker.Application.WeightMeasurements.Commands
{
	/// <summary>
	/// Klasa reprezentująca komendę usuwania pomiaru masy ciała.
	/// Wymaga podania zarówno identyfikatora pomiaru, jak i użytkownika w celu weryfikacji uprawnień.
	/// </summary>
	/// <remarks>
	/// Rekord typu positional record z parametrami konstruktora.
	/// </remarks>
	public record DeleteWeightMeasurementCommand(
		/// <summary>
		/// Identyfikator pomiaru do usunięcia.
		/// Wymagany parametr bez domyślnej wartości.
		/// </summary>
		Guid Id,

		/// <summary>
		/// Identyfikator użytkownika wykonującego operację.
		/// Wymagany parametr bez domyślnej wartości.
		/// </summary>
		string UserId);
}