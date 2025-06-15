// Plik ProductsController.cs - kontroler zarządzania produktami spożywczymi w aplikacji CalorieTracker.
// Odpowiada za operacje CRUD produktów, wyszukiwanie oraz zarządzanie bazą danych produktów spożywczych.

using AutoMapper;
using CalorieTracker.Api.Attributes;
using CalorieTracker.Api.Models.Products;
using CalorieTracker.Application.Auth.Commands;
using CalorieTracker.Application.Auth.Handlers;
using CalorieTracker.Application.Auth.Queries;
using CalorieTracker.Application.Products.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CalorieTracker.Api.Controllers
{
	/// <summary>
	/// Kontroler zarządzania produktami spożywczymi implementujący pełny cykl CRUD.
	/// Obsługuje tworzenie, odczytywanie, aktualizację i usuwanie produktów spożywczych.
	/// Wymaga autoryzacji oraz kompletnego profilu użytkownika dla wszystkich operacji.
	/// Implementuje wzorzec CQRS z dedykowanymi handlerami dla każdej operacji biznesowej.
	/// </summary>
	[Authorize]
	[RequireCompleteProfile]
	[ApiController]
	[Route("api/[controller]")]
	public class ProductsController : ControllerBase
	{
		/// <summary>
		/// Mapper AutoMapper do konwersji między modelami API a komendami/zapytaniami domenowymi.
		/// </summary>
		private readonly IMapper _mapper;

		/// <summary>
		/// Handler odpowiedzialny za tworzenie nowych produktów w systemie.
		/// </summary>
		private readonly CreateProductHandler _createHandler;

		/// <summary>
		/// Handler zarządzający aktualizacją istniejących produktów.
		/// </summary>
		private readonly UpdateProductHandler _updateHandler;

		/// <summary>
		/// Handler obsługujący usuwanie produktów z systemu.
		/// </summary>
		private readonly DeleteProductHandler _deleteHandler;

		/// <summary>
		/// Handler wyszukiwania produktów z filtrowaniem i paginacją.
		/// </summary>
		private readonly SearchProductsHandler _searchHandler;

		/// <summary>
		/// Handler pobierania szczegółów produktu na podstawie identyfikatora.
		/// </summary>
		private readonly GetProductByIdHandler _getByIdHandler;

		/// <summary>
		/// Handler wyszukiwania produktu na podstawie kodu kreskowego.
		/// </summary>
		private readonly GetProductByBarcodeHandler _getBarcodeHandler;

		/// <summary>
		/// Handler pobierania produktów należących do konkretnego użytkownika.
		/// </summary>
		private readonly GetUserProductsHandler _getUserProductsHandler;

		/// <summary>
		/// Inicjalizuje nową instancję kontrolera produktów z wymaganymi zależnościami.
		/// </summary>
		/// <param name="mapper">Mapper AutoMapper do konwersji obiektów.</param>
		/// <param name="createHandler">Handler tworzenia produktów.</param>
		/// <param name="updateHandler">Handler aktualizacji produktów.</param>
		/// <param name="deleteHandler">Handler usuwania produktów.</param>
		/// <param name="searchHandler">Handler wyszukiwania produktów.</param>
		/// <param name="getByIdHandler">Handler pobierania produktu po ID.</param>
		/// <param name="getBarcodeHandler">Handler pobierania produktu po kodzie kreskowym.</param>
		/// <param name="getUserProductsHandler">Handler pobierania produktów użytkownika.</param>
		public ProductsController(
			   IMapper mapper,
			   CreateProductHandler createHandler,
			   UpdateProductHandler updateHandler,
			   DeleteProductHandler deleteHandler,
			   SearchProductsHandler searchHandler,
			   GetProductByIdHandler getByIdHandler,
			   GetProductByBarcodeHandler getBarcodeHandler,
			   GetUserProductsHandler getUserProductsHandler)
		{
			_mapper = mapper;
			_createHandler = createHandler;
			_updateHandler = updateHandler;
			_deleteHandler = deleteHandler;
			_searchHandler = searchHandler;
			_getByIdHandler = getByIdHandler;
			_getBarcodeHandler = getBarcodeHandler;
			_getUserProductsHandler = getUserProductsHandler;
		}

		/// <summary>
		/// Wyszukuje produkty spożywcze na podstawie nazwy z opcjonalnym filtrowaniem.
		/// Obsługuje wyszukiwanie pełnotekstowe po nazwie i marce produktu oraz filtrowanie po kategorii.
		/// Implementuje paginację wyników dla lepszej wydajności.
		/// </summary>
		/// <param name="request">Parametry wyszukiwania zawierające frazę, opcjonalną kategorię i parametry paginacji.</param>
		/// <returns>
		/// 200 OK z listą produktów - wyniki wyszukiwania z informacją o paginacji.
		/// 400 BadRequest - gdy parametry wyszukiwania są nieprawidłowe.
		/// 401 Unauthorized - gdy użytkownik nie jest zalogowany.
		/// 403 Forbidden - gdy profil użytkownika nie jest kompletny.
		/// </returns>
		[HttpGet("search")]
		public async Task<ActionResult<SearchProductsResponse>> SearchProducts([FromQuery] SearchProductsRequest request)
		{
			// Konwertuje żądanie API na zapytanie domenowe
			var query = _mapper.Map<SearchProductsQuery>(request);

			// Wykonuje wyszukiwanie produktów
			var products = await _searchHandler.Handle(query);

			// Mapuje wyniki na DTO i przygotowuje odpowiedź z paginacją
			var productDtos = _mapper.Map<List<ProductSummaryDto>>(products);

			return Ok(new SearchProductsResponse
			{
				Products = productDtos,
				TotalCount = productDtos.Count,
				HasMore = productDtos.Count == request.Take
			});
		}

		/// <summary>
		/// Pobiera pełne szczegóły produktu na podstawie jego unikalnego identyfikatora.
		/// Zwraca kompletne informacje o produkcie włączając wszystkie wartości odżywcze.
		/// </summary>
		/// <param name="id">Unikalny identyfikator produktu (GUID).</param>
		/// <returns>
		/// 200 OK z pełnymi danymi produktu - gdy produkt został znaleziony.
		/// 404 NotFound - gdy produkt o podanym ID nie istnieje.
		/// 401 Unauthorized - gdy użytkownik nie jest zalogowany.
		/// 403 Forbidden - gdy profil użytkownika nie jest kompletny.
		/// </returns>
		[HttpGet("{id:guid}")]
		public async Task<ActionResult<ProductDto>> GetProduct(Guid id)
		{
			// Pobiera produkt z bazy danych
			var product = await _getByIdHandler.Handle(new GetProductByIdQuery(id));

			// Sprawdza czy produkt istnieje
			if (product is null)
				return NotFound("Produkt nie został znaleziony");

			// Konwertuje encję na DTO i zwraca wynik
			var productDto = _mapper.Map<ProductDto>(product);
			return Ok(productDto);
		}

		/// <summary>
		/// Wyszukuje produkt na podstawie jego kodu kreskowego.
		/// Umożliwia szybkie wyszukiwanie produktów przez skanowanie kodów kreskowych.
		/// </summary>
		/// <param name="barcode">Kod kreskowy produktu do wyszukania.</param>
		/// <returns>
		/// 200 OK z danymi produktu - gdy produkt z danym kodem został znaleziony.
		/// 404 NotFound - gdy produkt o podanym kodzie kreskowym nie istnieje.
		/// 401 Unauthorized - gdy użytkownik nie jest zalogowany.
		/// 403 Forbidden - gdy profil użytkownika nie jest kompletny.
		/// </returns>
		[HttpGet("barcode/{barcode}")]
		public async Task<ActionResult<ProductDto>> GetProductByBarcode(string barcode)
		{
			// Wyszukuje produkt po kodzie kreskowym
			var product = await _getBarcodeHandler.Handle(new GetProductByBarcodeQuery(barcode));

			// Weryfikuje czy produkt został znaleziony
			if (product is null)
				return NotFound("Produkt o tym kodzie kreskowym nie został znaleziony");

			// Mapuje i zwraca dane produktu
			var productDto = _mapper.Map<ProductDto>(product);
			return Ok(productDto);
		}

		/// <summary>
		/// Pobiera listę produktów utworzonych przez aktualnie zalogowanego użytkownika.
		/// Implementuje paginację dla efektywnego przeglądania dużych kolekcji produktów.
		/// Zwraca uproszczone informacje o produktach (bez pełnych szczegółów).
		/// </summary>
		/// <param name="skip">Liczba produktów do pominięcia (domyślnie 0).</param>
		/// <param name="take">Liczba produktów do pobrania (domyślnie 20, maksymalnie 100).</param>
		/// <returns>
		/// 200 OK z listą produktów użytkownika - produkty w formacie uproszczonym.
		/// 401 Unauthorized - gdy użytkownik nie jest zalogowany.
		/// 403 Forbidden - gdy profil użytkownika nie jest kompletny.
		/// </returns>
		[HttpGet("my-products")]
		public async Task<ActionResult<List<ProductSummaryDto>>> GetMyProducts([FromQuery] int skip = 0, [FromQuery] int take = 20)
		{
			// Ekstraktuje ID użytkownika z tokenu JWT
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId is null) return Unauthorized();

			// Tworzy zapytanie z parametrami paginacji
			var query = new GetUserProductsQuery { UserId = userId, Skip = skip, Take = take };

			// Pobiera produkty użytkownika
			var products = await _getUserProductsHandler.Handle(query);

			// Konwertuje na DTO i zwraca wyniki
			var productDtos = _mapper.Map<List<ProductSummaryDto>>(products);
			return Ok(productDtos);
		}

		/// <summary>
		/// Tworzy nowy produkt spożywczy w systemie.
		/// Automatycznie przypisuje utworzony produkt do aktualnie zalogowanego użytkownika.
		/// Zwraca lokalizację nowo utworzonego zasobu zgodnie z konwencją REST.
		/// </summary>
		/// <param name="request">Dane nowego produktu zawierające wszystkie wymagane informacje odżywcze.</param>
		/// <returns>
		/// 201 Created z ID nowego produktu i lokalizacją - gdy produkt został pomyślnie utworzony.
		/// 400 BadRequest - gdy dane produktu są nieprawidłowe lub nie przechodzą walidacji.
		/// 401 Unauthorized - gdy użytkownik nie jest zalogowany.
		/// 403 Forbidden - gdy profil użytkownika nie jest kompletny.
		/// </returns>
		[HttpPost]
		public async Task<ActionResult<Guid>> CreateProduct([FromBody] CreateProductRequest request)
		{
			// Weryfikuje autoryzację użytkownika
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId is null) return Unauthorized();

			// Mapuje żądanie na komendę domenową i przypisuje właściciela
			var command = _mapper.Map<CreateProductCommand>(request);
			command = command with { CreatedByUserId = userId };

			// Wykonuje operację tworzenia produktu
			var productId = await _createHandler.Handle(command);

			// Zwraca odpowiedź 201 Created z lokalizacją nowego zasobu
			return CreatedAtAction(
				   nameof(GetProduct),
				   new { id = productId },
				   new { id = productId });
		}

		/// <summary>
		/// Aktualizuje istniejący produkt spożywczy.
		/// Tylko właściciel produktu lub administrator może wykonać tę operację.
		/// Automatycznie aktualizuje znacznik czasu ostatniej modyfikacji.
		/// </summary>
		/// <param name="id">Unikalny identyfikator produktu do aktualizacji.</param>
		/// <param name="request">Nowe dane produktu zastępujące istniejące wartości.</param>
		/// <returns>
		/// 204 NoContent - gdy aktualizacja przebiegła pomyślnie.
		/// 400 BadRequest - gdy dane produktu są nieprawidłowe.
		/// 401 Unauthorized - gdy użytkownik nie jest zalogowany.
		/// 403 Forbidden - gdy profil użytkownika nie jest kompletny.
		/// 404 NotFound - gdy produkt nie istnieje lub użytkownik nie ma uprawnień do edycji.
		/// </returns>
		[HttpPut("{id:guid}")]
		public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductRequest request)
		{
			// Sprawdza autoryzację użytkownika
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId is null) return Unauthorized();

			// Przygotowuje komendę aktualizacji z danymi użytkownika
			var command = _mapper.Map<UpdateProductCommand>(request);
			command = command with { Id = id, UpdatedByUserId = userId };

			// Wykonuje operację aktualizacji z weryfikacją uprawnień
			var success = await _updateHandler.Handle(command);

			// Sprawdza czy operacja zakończyła się sukcesem
			if (!success)
				return NotFound("Produkt nie został znaleziony lub nie masz uprawnień do jego edycji");

			return NoContent();
		}

		/// <summary>
		/// Usuwa produkt spożywczy z systemu.
		/// Tylko właściciel produktu lub administrator może wykonać tę operację.
		/// Operacja jest nieodwracalna - usuwa produkt permanentnie z bazy danych.
		/// </summary>
		/// <param name="id">Unikalny identyfikator produktu do usunięcia.</param>
		/// <returns>
		/// 204 NoContent - gdy usunięcie przebiegło pomyślnie.
		/// 401 Unauthorized - gdy użytkownik nie jest zalogowany.
		/// 403 Forbidden - gdy profil użytkownika nie jest kompletny.
		/// 404 NotFound - gdy produkt nie istnieje lub użytkownik nie ma uprawnień do usunięcia.
		/// </returns>
		[HttpDelete("{id:guid}")]
		public async Task<IActionResult> DeleteProduct(Guid id)
		{
			// Weryfikuje tożsamość użytkownika
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId is null) return Unauthorized();

			// Wykonuje operację usunięcia z weryfikacją uprawnień
			var success = await _deleteHandler.Handle(new DeleteProductCommand(id, userId));

			// Sprawdza rezultat operacji
			if (!success)
				return NotFound("Produkt nie został znaleziony lub nie masz uprawnień do jego usunięcia");

			return NoContent();
		}
	}
}