// Plik SearchProductsQuery.cs - definicja zapytania o wyszukiwanie produktów
// Odpowiada za przenoszenie parametrów wyszukiwania i filtrowania produktów

using CalorieTracker.Domain.Enums;

namespace CalorieTracker.Application.Auth.Queries
{
	/// <summary>
	/// Klasa reprezentująca zapytanie o wyszukiwanie produktów z możliwością filtrowania i paginacji.
	/// Pozwala na wyszukiwanie produktów po nazwie, kategorii oraz statusie weryfikacji.
	/// </summary>
	/// <remarks>
	/// Właściwości init-only zapewniają niezmienność obiektu po inicjalizacji.
	/// Domyślne wartości parametrów paginacji (Skip=0, Take=20) implementują podstawową paginację.
	/// </remarks>
	public class SearchProductsQuery
	{
		/// <summary>
		/// Fraza do wyszukania w nazwach produktów
		/// </summary>
		/// <value>
		/// Ciąg znaków używany do wyszukiwania produktów zawierających podaną frazę.
		/// Wartość musi być różna od null.
		/// </value>
		public string SearchTerm { get; init; } = null!;

		/// <summary>
		/// Opcjonalny filtr kategorii produktu
		/// </summary>
		/// <value>
		/// Wartość enum <see cref="ProductCategory"/> określająca kategorię produktu.
		/// Może być null, co oznacza brak filtrowania po kategorii.
		/// </value>
		public ProductCategory? Category { get; init; }

		/// <summary>
		/// Liczba produktów do pominięcia (stronicowanie)
		/// </summary>
		/// <value>
		/// Domyślna wartość: 0. Określa offset w paginacji wyników.
		/// </value>
		public int Skip { get; init; } = 0;

		/// <summary>
		/// Maksymalna liczba produktów do pobrania (stronicowanie)
		/// </summary>
		/// <value>
		/// Domyślna wartość: 20. Określa rozmiar strony wyników.
		/// </value>
		public int Take { get; init; } = 20;

		/// <summary>
		/// Flaga określająca czy zwracać tylko zweryfikowane produkty
		/// </summary>
		/// <value>
		/// Domyślna wartość: false. Gdy true, wyniki zawierają tylko produkty oznaczone jako zweryfikowane.
		/// </value>
		public bool VerifiedOnly { get; init; } = false;
	}
}