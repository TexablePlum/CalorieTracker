// Plik GetProductByBarcodeQuery.cs - definicja zapytania o pobranie produktu po kodzie kreskowym.
// Odpowiada za przenoszenie danych potrzebnych do wyszukania produktu na podstawie kodu kreskowego.

namespace CalorieTracker.Application.Auth.Queries
{
	/// <summary>
	/// Rekord reprezentujący zapytanie o pobranie produktu na podstawie kodu kreskowego.
	/// Przenosi dane potrzebne do identyfikacji produktu w systemie.
	/// </summary>
	/// <remarks>
	/// Wykorzystuje typ record, który jest niezmienny (immutable) i idealny do przenoszenia danych zapytań.
	/// </remarks>
	public record GetProductByBarcodeQuery(string Barcode)
	{
		/// <summary>
		/// Kod kreskowy produktu do wyszukania.
		/// </summary>
		/// <value>
		/// Ciąg znaków reprezentujący unikalny identyfikator produktu w systemie kodów kreskowych.
		/// </value>
		public string Barcode { get; init; } = Barcode;
	}
}