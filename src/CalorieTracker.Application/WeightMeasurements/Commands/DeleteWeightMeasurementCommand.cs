// Plik DeleteWeightMeasurementCommand.cs - definicja komendy usuwania pomiaru wagi.
// Reprezentuje żądanie usunięcia istniejącego pomiaru masy ciała użytkownika.

namespace CalorieTracker.Application.WeightMeasurements.Commands
{
	/// <summary>
	/// Klasa reprezentująca komendę usuwania pomiaru masy ciała.
	/// Wymaga podania zarówno identyfikatora pomiaru, jak i użytkownika w celu weryfikacji uprawnień.
	/// </summary>
	/// <param name="Id">
	/// Identyfikator pomiaru do usunięcia.
	/// Wymagany parametr bez domyślnej wartości.
	/// </param>
	/// <param name="UserId">
	/// Identyfikator użytkownika wykonującego operację.
	/// Wymagany parametr bez domyślnej wartości.
	/// </param>
	/// <remarks>
	/// Rekord typu positional record z parametrami konstruktora.
	/// </remarks>
	public record DeleteWeightMeasurementCommand(Guid Id, string UserId);
}