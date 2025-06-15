// Plik GetProductByIdQuery.cs - definicja zapytania o pobranie produktu po identyfikatorze.
// Odpowiada za przenoszenie danych potrzebnych do wyszukania produktu na podstawie unikalnego ID.

namespace CalorieTracker.Application.Auth.Queries
{
	/// <summary>
	/// Rekord reprezentujący zapytanie o pobranie produktu na podstawie jego identyfikatora.
	/// Przenosi dane potrzebne do jednoznacznej identyfikacji produktu w systemie.
	/// </summary>
	/// <remarks>
	/// Wykorzystuje typ record, który zapewnia niezmienność (immutability) i jest optymalny 
	/// dla obiektów transferu danych (DTO). Identyfikator produktu jest typu Guid, co gwarantuje
	/// jego unikalność w systemie.
	/// </remarks>
	public record GetProductByIdQuery(Guid Id)
	{
		/// <summary>
		/// Unikalny identyfikator produktu do wyszukania.
		/// </summary>
		/// <value>
		/// Wartość Guid reprezentująca unikalny klucz produktu w systemie.
		/// </value>
		public Guid Id { get; init; } = Id;
	}
}