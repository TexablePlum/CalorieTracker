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
	/// </summary>
	public class RecipeMappingProfile : Profile
	{
		/// <summary>
		/// Inicjalizuje nową instancję profilu mapowania dla przepisów.
		/// Konfiguruje wszystkie mapowania wymagane do przetwarzania operacji na przepisach.
		/// </summary>
		public RecipeMappingProfile()
		{
			/// <summary>
			/// Mapowanie z <see cref="CreateRecipeRequest"/> na <see cref="CreateRecipeCommand"/>.
			/// Transformuje model żądania utworzenia przepisu na odpowiednią komendę.
			/// Pomija pole CreatedByUserId, które jest ustawiane w kontrolerze.
			/// </summary>
			CreateMap<CreateRecipeRequest, CreateRecipeCommand>()
				.ForMember(dest => dest.CreatedByUserId, opt => opt.Ignore());

			/// <summary>
			/// Mapowanie z <see cref="CreateRecipeIngredientRequest"/> na <see cref="CreateRecipeIngredientCommand"/>.
			/// Transformuje model składnika przepisu na odpowiednią komendę.
			/// </summary>
			CreateMap<CreateRecipeIngredientRequest, CreateRecipeIngredientCommand>();

			/// <summary>
			/// Mapowanie z <see cref="UpdateRecipeRequest"/> na <see cref="UpdateRecipeCommand"/>.
			/// Transformuje model żądania aktualizacji przepisu na odpowiednią komendę.
			/// Pomija pola Id i UpdatedByUserId, które są ustawiane w kontrolerze.
			/// </summary>
			CreateMap<UpdateRecipeRequest, UpdateRecipeCommand>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.UpdatedByUserId, opt => opt.Ignore());

			/// <summary>
			/// Mapowanie z <see cref="SearchRecipesRequest"/> na <see cref="SearchRecipesQuery"/>.
			/// Transformuje parametry wyszukiwania przepisów na odpowiednie zapytanie.
			/// </summary>
			CreateMap<SearchRecipesRequest, SearchRecipesQuery>();

			/// <summary>
			/// Mapowanie z encji <see cref="Recipe"/> na model <see cref="RecipeDto"/>.
			/// Transformuje pełne dane przepisu na format odpowiedzi API.
			/// Pomija pole TotalNutrition, które jest obliczane osobno przez serwis domenowy.
			/// </summary>
			CreateMap<Recipe, RecipeDto>()
				.ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.Ingredients))
				.ForMember(dest => dest.TotalNutrition, opt => opt.Ignore());

			/// <summary>
			/// Mapowanie z encji <see cref="RecipeIngredient"/> na model <see cref="RecipeIngredientDto"/>.
			/// Transformuje dane składnika przepisu na format odpowiedzi API.
			/// Zawiera dodatkowe informacje o produkcie (nazwa, kategoria, jednostka).
			/// </summary>
			CreateMap<RecipeIngredient, RecipeIngredientDto>()
				.ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
				.ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
				.ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Product.Category.ToString()))
				.ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Product.Unit.ToString()));

			/// <summary>
			/// Mapowanie z obiektu wartości <see cref="RecipeNutrition"/> na model <see cref="RecipeNutritionDto"/>.
			/// Transformuje wartości odżywcze przepisu na format odpowiedzi API.
			/// </summary>
			CreateMap<RecipeNutrition, RecipeNutritionDto>();

			/// <summary>
			/// Mapowanie z encji <see cref="Recipe"/> na model <see cref="RecipeSummaryDto"/>.
			/// Transformuje podstawowe dane przepisu na uproszczony format odpowiedzi API.
			/// Pomija pole TotalCalories, które jest obliczane osobno przez serwis domenowy.
			/// </summary>
			CreateMap<Recipe, RecipeSummaryDto>()
				.ForMember(dest => dest.TotalCalories, opt => opt.Ignore());
		}
	}
}