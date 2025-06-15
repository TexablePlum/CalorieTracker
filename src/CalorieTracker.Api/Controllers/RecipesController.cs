// Plik RecipesController.cs - kontroler zarządzania przepisami kulinarnymi w aplikacji CalorieTracker.
// Odpowiada za operacje CRUD przepisów, wyszukiwanie oraz automatyczne kalkulacje wartości odżywczych.

using AutoMapper;
using CalorieTracker.Api.Attributes;
using CalorieTracker.Api.Models.Recipes;
using CalorieTracker.Application.Recipes.Commands;
using CalorieTracker.Application.Recipes.Handlers;
using CalorieTracker.Application.Recipes.Queries;
using CalorieTracker.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CalorieTracker.Api.Controllers
{
	/// <summary>
	/// Kontroler przepisów kulinarnych zarządzający recepturami i ich wartościami odżywczymi.
	/// Obsługuje pełny cykl CRUD przepisów z automatyczną kalkulacją wartości odżywczych na podstawie składników.
	/// Wymaga autoryzacji oraz kompletnego profilu użytkownika dla wszystkich operacji.
	/// Implementuje wzorzec CQRS z dedykowanymi handlerami i integruje serwis kalkulacji wartości odżywczych.
	/// </summary>
	[Authorize]
	[RequireCompleteProfile]
	[ApiController]
	[Route("api/[controller]")]
	public class RecipesController : ControllerBase
	{
		/// <summary>
		/// Mapper AutoMapper do konwersji między modelami API a komendami/zapytaniami domenowymi.
		/// </summary>
		private readonly IMapper _mapper;

		/// <summary>
		/// Serwis kalkulacji wartości odżywczych przepisów na podstawie składników i ich proporcji.
		/// </summary>
		private readonly RecipeNutritionCalculator _nutritionCalculator;

		/// <summary>
		/// Handler odpowiedzialny za tworzenie nowych przepisów w systemie.
		/// </summary>
		private readonly CreateRecipeHandler _createHandler;

		/// <summary>
		/// Handler zarządzający aktualizacją istniejących przepisów kulinarnych.
		/// </summary>
		private readonly UpdateRecipeHandler _updateHandler;

		/// <summary>
		/// Handler obsługujący usuwanie przepisów z systemu.
		/// </summary>
		private readonly DeleteRecipeHandler _deleteHandler;

		/// <summary>
		/// Handler pobierania szczegółowych informacji o przepisie wraz ze składnikami.
		/// </summary>
		private readonly GetRecipeDetailsHandler _getDetailsHandler;

		/// <summary>
		/// Handler wyszukiwania przepisów z filtrowaniem i paginacją.
		/// </summary>
		private readonly SearchRecipesHandler _searchHandler;

		/// <summary>
		/// Handler pobierania przepisów należących do konkretnego użytkownika.
		/// </summary>
		private readonly GetUserRecipesHandler _getUserRecipesHandler;

		/// <summary>
		/// Handler pobierania wszystkich przepisów dostępnych w systemie.
		/// </summary>
		private readonly GetAllRecipesHandler _getAllRecipesHandler;

		/// <summary>
		/// Inicjalizuje nową instancję kontrolera przepisów z wymaganymi zależnościami.
		/// </summary>
		/// <param name="mapper">Mapper AutoMapper do konwersji obiektów.</param>
		/// <param name="nutritionCalculator">Serwis kalkulacji wartości odżywczych przepisów.</param>
		/// <param name="createHandler">Handler tworzenia przepisów.</param>
		/// <param name="updateHandler">Handler aktualizacji przepisów.</param>
		/// <param name="deleteHandler">Handler usuwania przepisów.</param>
		/// <param name="getDetailsHandler">Handler pobierania szczegółów przepisu.</param>
		/// <param name="searchHandler">Handler wyszukiwania przepisów.</param>
		/// <param name="getUserRecipesHandler">Handler pobierania przepisów użytkownika.</param>
		/// <param name="getAllRecipesHandler">Handler pobierania wszystkich przepisów.</param>
		public RecipesController(
			   IMapper mapper,
			   RecipeNutritionCalculator nutritionCalculator,
			   CreateRecipeHandler createHandler,
			   UpdateRecipeHandler updateHandler,
			   DeleteRecipeHandler deleteHandler,
			   GetRecipeDetailsHandler getDetailsHandler,
			   SearchRecipesHandler searchHandler,
			   GetUserRecipesHandler getUserRecipesHandler,
			   GetAllRecipesHandler getAllRecipesHandler)
		{
			_mapper = mapper;
			_nutritionCalculator = nutritionCalculator;
			_createHandler = createHandler;
			_updateHandler = updateHandler;
			_deleteHandler = deleteHandler;
			_getDetailsHandler = getDetailsHandler;
			_searchHandler = searchHandler;
			_getUserRecipesHandler = getUserRecipesHandler;
			_getAllRecipesHandler = getAllRecipesHandler;
		}

		/// <summary>
		/// Pobiera globalną listę wszystkich przepisów dostępnych w systemie.
		/// Automatycznie kalkuluje łączne kalorie dla każdego przepisu na podstawie składników.
		/// Implementuje paginację dla efektywnego przeglądania dużych kolekcji przepisów.
		/// </summary>
		/// <param name="skip">Liczba przepisów do pominięcia (domyślnie 0).</param>
		/// <param name="take">Liczba przepisów do pobrania (domyślnie 20, maksymalnie 100).</param>
		/// <returns>
		/// 200 OK z listą przepisów - przepisy w formacie uproszczonym z kaloriami.
		/// 401 Unauthorized - gdy użytkownik nie jest zalogowany.
		/// 403 Forbidden - gdy profil użytkownika nie jest kompletny.
		/// </returns>
		[HttpGet]
		public async Task<ActionResult<List<RecipeSummaryDto>>> GetAllRecipes([FromQuery] int skip = 0, [FromQuery] int take = 20)
		{
			// Pobiera przepisy z bazy danych z paginacją
			var query = new GetAllRecipesQuery { Skip = skip, Take = take };
			var recipes = await _getAllRecipesHandler.Handle(query);

			// Konwertuje przepisy na DTO z automatyczną kalkulacją kalorii
			var recipeDtos = new List<RecipeSummaryDto>();
			foreach (var recipe in recipes)
			{
				var dto = _mapper.Map<RecipeSummaryDto>(recipe);

				// Kalkuluje łączne kalorie przepisu na podstawie składników
				var nutrition = _nutritionCalculator.CalculateForRecipe(recipe);
				dto.TotalCalories = nutrition.Calories;

				recipeDtos.Add(dto);
			}

			return Ok(recipeDtos);
		}

		/// <summary>
		/// Wyszukuje przepisy kulinarne na podstawie nazwy z automatyczną kalkulacją wartości odżywczych.
		/// Obsługuje wyszukiwanie pełnotekstowe po nazwie przepisu i opcjonalnie po składnikach.
		/// Implementuje paginację wyników dla lepszej wydajności.
		/// </summary>
		/// <param name="request">Parametry wyszukiwania zawierające frazę i parametry paginacji.</param>
		/// <returns>
		/// 200 OK z wynikami wyszukiwania - lista przepisów z informacją o paginacji i kaloriach.
		/// 400 BadRequest - gdy parametry wyszukiwania są nieprawidłowe.
		/// 401 Unauthorized - gdy użytkownik nie jest zalogowany.
		/// 403 Forbidden - gdy profil użytkownika nie jest kompletny.
		/// </returns>
		[HttpGet("search")]
		public async Task<ActionResult<SearchRecipesResponse>> SearchRecipes([FromQuery] SearchRecipesRequest request)
		{
			// Konwertuje żądanie API na zapytanie domenowe
			var query = _mapper.Map<SearchRecipesQuery>(request);
			var recipes = await _searchHandler.Handle(query);

			// Przetwarza wyniki wyszukiwania z kalkulacją wartości odżywczych
			var recipeDtos = new List<RecipeSummaryDto>();
			foreach (var recipe in recipes)
			{
				var dto = _mapper.Map<RecipeSummaryDto>(recipe);

				// Automatycznie kalkuluje kalorie dla każdego znalezionego przepisu
				var nutrition = _nutritionCalculator.CalculateForRecipe(recipe);
				dto.TotalCalories = nutrition.Calories;

				recipeDtos.Add(dto);
			}

			return Ok(new SearchRecipesResponse
			{
				Recipes = recipeDtos,
				TotalCount = recipeDtos.Count,
				HasMore = recipeDtos.Count == request.Take
			});
		}

		/// <summary>
		/// Pobiera listę przepisów utworzonych przez aktualnie zalogowanego użytkownika.
		/// Implementuje paginację i automatyczną kalkulację kalorii dla każdego przepisu.
		/// Zwraca uproszczone informacje o przepisach użytkownika (bez pełnych szczegółów składników).
		/// </summary>
		/// <param name="skip">Liczba przepisów do pominięcia (domyślnie 0).</param>
		/// <param name="take">Liczba przepisów do pobrania (domyślnie 20, maksymalnie 100).</param>
		/// <returns>
		/// 200 OK z listą przepisów użytkownika - przepisy w formacie uproszczonym z kaloriami.
		/// 401 Unauthorized - gdy użytkownik nie jest zalogowany.
		/// 403 Forbidden - gdy profil użytkownika nie jest kompletny.
		/// </returns>
		[HttpGet("my")]
		public async Task<ActionResult<List<RecipeSummaryDto>>> GetMyRecipes([FromQuery] int skip = 0, [FromQuery] int take = 20)
		{
			// Ekstraktuje ID użytkownika z tokenu JWT
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId is null) return Unauthorized();

			// Pobiera przepisy należące do użytkownika
			var query = new GetUserRecipesQuery { UserId = userId, Skip = skip, Take = take };
			var recipes = await _getUserRecipesHandler.Handle(query);

			// Przetwarza przepisy użytkownika z kalkulacją wartości odżywczych
			var recipeDtos = new List<RecipeSummaryDto>();
			foreach (var recipe in recipes)
			{
				var dto = _mapper.Map<RecipeSummaryDto>(recipe);

				// Kalkuluje łączne kalorie przepisu na podstawie wszystkich składników
				var nutrition = _nutritionCalculator.CalculateForRecipe(recipe);
				dto.TotalCalories = nutrition.Calories;

				recipeDtos.Add(dto);
			}

			return Ok(recipeDtos);
		}

		/// <summary>
		/// Pobiera pełne szczegóły przepisu kulinarnego wraz ze składnikami i wartościami odżywczymi.
		/// Automatycznie kalkuluje kompletne informacje nutrityczne na podstawie wszystkich składników.
		/// Zwraca szczegółowe informacje including instrukcje przygotowania i kompletną analizę odżywczą.
		/// </summary>
		/// <param name="id">Unikalny identyfikator przepisu (GUID).</param>
		/// <returns>
		/// 200 OK z pełnymi danymi przepisu - kompletne informacje o przepisie i wartościach odżywczych.
		/// 404 NotFound - gdy przepis o podanym ID nie istnieje.
		/// 401 Unauthorized - gdy użytkownik nie jest zalogowany.
		/// 403 Forbidden - gdy profil użytkownika nie jest kompletny.
		/// </returns>
		[HttpGet("{id:guid}")]
		public async Task<ActionResult<RecipeDto>> GetRecipe(Guid id)
		{
			// Pobiera szczegóły przepisu z bazy danych
			var recipe = await _getDetailsHandler.Handle(new GetRecipeDetailsQuery(id));

			// Sprawdza czy przepis istnieje
			if (recipe is null)
				return NotFound("Przepis nie został znaleziony");

			// Konwertuje na DTO i kalkuluje kompletne wartości odżywcze
			var recipeDto = _mapper.Map<RecipeDto>(recipe);
			var nutrition = _nutritionCalculator.CalculateForRecipe(recipe);
			recipeDto.TotalNutrition = _mapper.Map<RecipeNutritionDto>(nutrition);

			return Ok(recipeDto);
		}

		/// <summary>
		/// Tworzy nowy przepis kulinarny w systemie.
		/// Automatycznie przypisuje utworzony przepis do aktualnie zalogowanego użytkownika.
		/// Zwraca lokalizację nowo utworzonego zasobu zgodnie z konwencją REST.
		/// </summary>
		/// <param name="request">Dane nowego przepisu zawierające nazwę, składniki i instrukcje przygotowania.</param>
		/// <returns>
		/// 201 Created z ID nowego przepisu i lokalizacją - gdy przepis został pomyślnie utworzony.
		/// 400 BadRequest - gdy dane przepisu są nieprawidłowe lub nie przechodzą walidacji.
		/// 401 Unauthorized - gdy użytkownik nie jest zalogowany.
		/// 403 Forbidden - gdy profil użytkownika nie jest kompletny.
		/// </returns>
		[HttpPost]
		public async Task<ActionResult<Guid>> CreateRecipe([FromBody] CreateRecipeRequest request)
		{
			// Weryfikuje autoryzację użytkownika
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId is null) return Unauthorized();

			// Mapuje żądanie na komendę domenową i przypisuje właściciela
			var command = _mapper.Map<CreateRecipeCommand>(request);
			command = command with { CreatedByUserId = userId };

			// Wykonuje operację tworzenia przepisu
			var recipeId = await _createHandler.Handle(command);

			// Zwraca odpowiedź 201 Created z lokalizacją nowego zasobu
			return CreatedAtAction(
				   nameof(GetRecipe),
				   new { id = recipeId },
				   new { id = recipeId });
		}

		/// <summary>
		/// Aktualizuje istniejący przepis kulinarny.
		/// Tylko właściciel przepisu lub administrator może wykonać tę operację.
		/// Automatycznie aktualizuje znacznik czasu ostatniej modyfikacji.
		/// </summary>
		/// <param name="id">Unikalny identyfikator przepisu do aktualizacji.</param>
		/// <param name="request">Nowe dane przepisu zastępujące istniejące wartości.</param>
		/// <returns>
		/// 204 NoContent - gdy aktualizacja przebiegła pomyślnie.
		/// 400 BadRequest - gdy dane przepisu są nieprawidłowe.
		/// 401 Unauthorized - gdy użytkownik nie jest zalogowany.
		/// 403 Forbidden - gdy profil użytkownika nie jest kompletny.
		/// 404 NotFound - gdy przepis nie istnieje lub użytkownik nie ma uprawnień do edycji.
		/// </returns>
		[HttpPut("{id:guid}")]
		public async Task<IActionResult> UpdateRecipe(Guid id, [FromBody] UpdateRecipeRequest request)
		{
			// Sprawdza autoryzację użytkownika
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId is null) return Unauthorized();

			// Przygotowuje komendę aktualizacji z danymi użytkownika
			var command = _mapper.Map<UpdateRecipeCommand>(request);
			command = command with { Id = id, UpdatedByUserId = userId };

			// Wykonuje operację aktualizacji z weryfikacją uprawnień
			var success = await _updateHandler.Handle(command);

			// Sprawdza czy operacja zakończyła się sukcesem
			if (!success)
				return NotFound("Przepis nie został znaleziony lub nie masz uprawnień do jego edycji");

			return NoContent();
		}

		/// <summary>
		/// Usuwa przepis kulinarny z systemu.
		/// Tylko właściciel przepisu lub administrator może wykonać tę operację.
		/// Operacja jest nieodwracalna - usuwa przepis permanentnie wraz ze wszystkimi składnikami.
		/// </summary>
		/// <param name="id">Unikalny identyfikator przepisu do usunięcia.</param>
		/// <returns>
		/// 204 NoContent - gdy usunięcie przebiegło pomyślnie.
		/// 401 Unauthorized - gdy użytkownik nie jest zalogowany.
		/// 403 Forbidden - gdy profil użytkownika nie jest kompletny.
		/// 404 NotFound - gdy przepis nie istnieje lub użytkownik nie ma uprawnień do usunięcia.
		/// </returns>
		[HttpDelete("{id:guid}")]
		public async Task<IActionResult> DeleteRecipe(Guid id)
		{
			// Weryfikuje tożsamość użytkownika
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId is null) return Unauthorized();

			// Wykonuje operację usunięcia z weryfikacją uprawnień
			var success = await _deleteHandler.Handle(new DeleteRecipeCommand(id, userId));

			// Sprawdza rezultat operacji
			if (!success)
				return NotFound("Przepis nie został znaleziony lub nie masz uprawnień do jego usunięcia");

			return NoContent();
		}
	}
}