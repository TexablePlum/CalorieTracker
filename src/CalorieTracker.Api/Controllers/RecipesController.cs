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
	/// Kontroler do zarządzania przepisami kulinarnymi
	/// </summary>
	[Authorize]
	[RequireCompleteProfile]
	[ApiController]
	[Route("api/[controller]")]
	public class RecipesController : ControllerBase
	{
		private readonly IMapper _mapper;
		private readonly RecipeNutritionCalculator _nutritionCalculator;
		private readonly CreateRecipeHandler _createHandler;
		private readonly UpdateRecipeHandler _updateHandler;
		private readonly DeleteRecipeHandler _deleteHandler;
		private readonly GetRecipeDetailsHandler _getDetailsHandler;
		private readonly SearchRecipesHandler _searchHandler;
		private readonly GetUserRecipesHandler _getUserRecipesHandler;
		private readonly GetAllRecipesHandler _getAllRecipesHandler;

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
		/// Pobiera wszystkie przepisy (globalna lista)
		/// </summary>
		[HttpGet]
		public async Task<ActionResult<List<RecipeSummaryDto>>> GetAllRecipes([FromQuery] int skip = 0, [FromQuery] int take = 20)
		{
			var query = new GetAllRecipesQuery { Skip = skip, Take = take };
			var recipes = await _getAllRecipesHandler.Handle(query);

			var recipeDtos = new List<RecipeSummaryDto>();
			foreach (var recipe in recipes)
			{
				var dto = _mapper.Map<RecipeSummaryDto>(recipe);
				var nutrition = _nutritionCalculator.CalculateForRecipe(recipe);
				dto.TotalCalories = nutrition.Calories;
				recipeDtos.Add(dto);
			}

			return Ok(recipeDtos);
		}

		/// <summary>
		/// Wyszukuje przepisy po nazwie
		/// </summary>
		[HttpGet("search")]
		public async Task<ActionResult<SearchRecipesResponse>> SearchRecipes([FromQuery] SearchRecipesRequest request)
		{
			var query = _mapper.Map<SearchRecipesQuery>(request);
			var recipes = await _searchHandler.Handle(query);

			var recipeDtos = new List<RecipeSummaryDto>();
			foreach (var recipe in recipes)
			{
				var dto = _mapper.Map<RecipeSummaryDto>(recipe);
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
		/// Pobiera przepisy utworzone przez aktualnie zalogowanego użytkownika
		/// </summary>
		[HttpGet("my")]
		public async Task<ActionResult<List<RecipeSummaryDto>>> GetMyRecipes([FromQuery] int skip = 0, [FromQuery] int take = 20)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId is null) return Unauthorized();

			var query = new GetUserRecipesQuery { UserId = userId, Skip = skip, Take = take };
			var recipes = await _getUserRecipesHandler.Handle(query);

			var recipeDtos = new List<RecipeSummaryDto>();
			foreach (var recipe in recipes)
			{
				var dto = _mapper.Map<RecipeSummaryDto>(recipe);
				var nutrition = _nutritionCalculator.CalculateForRecipe(recipe);
				dto.TotalCalories = nutrition.Calories;
				recipeDtos.Add(dto);
			}

			return Ok(recipeDtos);
		}

		/// <summary>
		/// Pobiera szczegóły przepisu po ID
		/// </summary>
		[HttpGet("{id:guid}")]
		public async Task<ActionResult<RecipeDto>> GetRecipe(Guid id)
		{
			var recipe = await _getDetailsHandler.Handle(new GetRecipeDetailsQuery(id));

			if (recipe is null)
				return NotFound("Przepis nie został znaleziony");

			var recipeDto = _mapper.Map<RecipeDto>(recipe);
			var nutrition = _nutritionCalculator.CalculateForRecipe(recipe);
			recipeDto.TotalNutrition = _mapper.Map<RecipeNutritionDto>(nutrition);

			return Ok(recipeDto);
		}

		/// <summary>
		/// Tworzy nowy przepis
		/// </summary>
		[HttpPost]
		public async Task<ActionResult<Guid>> CreateRecipe([FromBody] CreateRecipeRequest request)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId is null) return Unauthorized();

			var command = _mapper.Map<CreateRecipeCommand>(request);
			command = command with { CreatedByUserId = userId };

			var recipeId = await _createHandler.Handle(command);

			return CreatedAtAction(
				nameof(GetRecipe),
				new { id = recipeId },
				new { id = recipeId });
		}

		/// <summary>
		/// Aktualizuje istniejący przepis (tylko właściciel może edytować)
		/// </summary>
		[HttpPut("{id:guid}")]
		public async Task<IActionResult> UpdateRecipe(Guid id, [FromBody] UpdateRecipeRequest request)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId is null) return Unauthorized();

			var command = _mapper.Map<UpdateRecipeCommand>(request);
			command = command with { Id = id, UpdatedByUserId = userId };

			var success = await _updateHandler.Handle(command);

			if (!success)
				return NotFound("Przepis nie został znaleziony lub nie masz uprawnień do jego edycji");

			return NoContent();
		}

		/// <summary>
		/// Usuwa przepis (tylko właściciel może usunąć)
		/// </summary>
		[HttpDelete("{id:guid}")]
		public async Task<IActionResult> DeleteRecipe(Guid id)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId is null) return Unauthorized();

			var success = await _deleteHandler.Handle(new DeleteRecipeCommand(id, userId));

			if (!success)
				return NotFound("Przepis nie został znaleziony lub nie masz uprawnień do jego usunięcia");

			return NoContent();
		}
	}
}