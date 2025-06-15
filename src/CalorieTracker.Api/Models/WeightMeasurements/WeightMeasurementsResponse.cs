// Plik WeightMeasurements.cs - modele transferu danych (DTO) związane z pomiarami masy ciała.
// Odpowiada za zwracanie paginowanej listy pomiarów wagi użytkownika przez API.
namespace CalorieTracker.Api.Models.WeightMeasurements
{
	/// <summary>
	/// Model odpowiedzi zawierający paginowaną listę pomiarów masy ciała.
	/// Używany jako odpowiedź API dla endpointów zwracających wiele pomiarów z podziałem na strony.
	/// </summary>
	public class WeightMeasurementsResponse
	{
		/// <summary>
		/// Lista pomiarów wagi dla aktualnej strony wyników.
		/// Każdy element zawiera pełne dane pomiaru w formacie <see cref="WeightMeasurementDto"/>.
		/// Domyślnie pusta lista.
		/// </summary>
		public List<WeightMeasurementDto> Measurements { get; set; } = new();

		/// <summary>
		/// Całkowita liczba pomiarów wagi dostępnych w systemie (niezależnie od paginacji).
		/// </summary>
		public int TotalCount { get; set; }

		/// <summary>
		/// Flaga wskazująca czy istnieją kolejne strony wyników do załadowania.
		/// Wartość true oznacza, że można wykonać kolejne żądanie z parametrem Skip,
		/// aby pobrać następną partię wyników.
		/// </summary>
		public bool HasMore { get; set; }
	}
}