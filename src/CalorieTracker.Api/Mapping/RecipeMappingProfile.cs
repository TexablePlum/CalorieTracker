// Plik RecipeMappingProfile.cs - konfiguracja mapowania obiektów dla modułu przepisów.
// Odpowiada za transformację między modelami API, komendami/zapytaniami a encjami domenowymi.
using AutoMapper;
using CalorieTracker.Api.Models.Recipes;
using CalorieTracker.Application.Recipes.Commands;
using CalorieTracker.Application.Recipes.Queries;
using CalorieTracker.Domain.Entities;
using CalorieTracker.Domain.ValueObjects;

namespace CalorieTracker.Api.Mapping
{
	/// <summary>
	/// Klasa profilu AutoMappera dla operacji związanych z przepisami.
	/// Definiuje mapowania między modelami API a obiektami komend, zapytań i encji domenowych.
	/// Obsługuje transformacje dla operacji CRUD na przepisach oraz kalkulacji wartości odżywczych.
	/// </summary>
	public class RecipeMappingProfile : Profile
	{
		/// <summary>
		/// Inicjalizuje nową instancję profilu mapowania dla przepisów.
		/// Konfiguruje mapowania dla:
		/// - Komendy tworzenia i aktualizacji przepisów
		/// - Zapytania wyszukiwania przepisów
		/// - Transformacje encji na DTO z obsługą składników i wartości odżywczych
		/// </summary>
		public RecipeMappingProfile()
		{
			ConfigureCreateRecipeMapping();
			ConfigureUpdateRecipeMapping();
			ConfigureSearchRecipeMapping();
			ConfigureRecipeToDto();
			ConfigureIngredientMapping();
			ConfigureNutritionMapping();
			ConfigureRecipeToSummary();
		}

		/// <summary>
		/// Konfiguruje mapowanie z CreateRecipeRequest na CreateRecipeCommand.
		/// Pomija pole CreatedByUserId, które jest ustawiane w kontrolerze.
		/// </summary>
		private void ConfigureCreateRecipeMapping()
		{
			CreateMap<CreateRecipeRequest, CreateRecipeCommand>()
				.ForMember(dest => dest.CreatedByUserId, opt => opt.Ignore());

			CreateMap<CreateRecipeIngredientRequest, CreateRecipeIngredientCommand>();
		}

		/// <summary>
		/// Konfiguruje mapowanie z UpdateRecipeRequest na UpdateRecipeCommand.
		/// Pomija pola Id i UpdatedByUserId, które są ustawiane w kontrolerze.
		/// </summary>
		private void ConfigureUpdateRecipeMapping()
		{
			CreateMap<UpdateRecipeRequest, UpdateRecipeCommand>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.UpdatedByUserId, opt => opt.Ignore());
		}

		/// <summary>
		/// Konfiguruje mapowanie z SearchRecipesRequest na SearchRecipesQuery.
		/// Transformuje parametry wyszukiwania przepisów na odpowiednie zapytanie.
		/// </summary>
		private void ConfigureSearchRecipeMapping()
		{
			CreateMap<SearchRecipesRequest, SearchRecipesQuery>();
		}

		/// <summary>
		/// Konfiguruje mapowanie z encji Recipe na model RecipeDto.
		/// Pomija pole TotalNutrition, które jest obliczane osobno przez serwis domenowy.
		/// </summary>
		private void ConfigureRecipeToDto()
		{
			CreateMap<Recipe, RecipeDto>()
				.ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.Ingredients))
				.ForMember(dest => dest.TotalNutrition, opt => opt.Ignore());
		}

		/// <summary>
		/// Konfiguruje mapowanie z encji RecipeIngredient na model RecipeIngredientDto.
		/// Zawiera dodatkowe informacje o produkcie (nazwa, kategoria, jednostka) z konwersją enum na string.
		/// </summary>
		private void ConfigureIngredientMapping()
		{
			CreateMap<RecipeIngredient, RecipeIngredientDto>()
				.ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
				.ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
				.ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Product.Category.ToString()))
				.ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Product.Unit.ToString()));
		}

		/// <summary>
		/// Konfiguruje mapowanie z obiektu wartości RecipeNutrition na model RecipeNutritionDto.
		/// Transformuje wartości odżywcze przepisu na format odpowiedzi API.
		/// </summary>
		private void ConfigureNutritionMapping()
		{
			CreateMap<RecipeNutrition, RecipeNutritionDto>();
		}

		/// <summary>
		/// Konfiguruje mapowanie z encji Recipe na model RecipeSummaryDto.
		/// Pomija pole TotalCalories, które jest obliczane osobno przez serwis domenowy.
		/// </summary>
		private void ConfigureRecipeToSummary()
		{
			CreateMap<Recipe, RecipeSummaryDto>()
				.ForMember(dest => dest.TotalCalories, opt => opt.Ignore());
		}
	}
}