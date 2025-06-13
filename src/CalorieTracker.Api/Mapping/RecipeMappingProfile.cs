using AutoMapper;
using CalorieTracker.Api.Models.Recipes;
using CalorieTracker.Application.Recipes.Commands;
using CalorieTracker.Application.Recipes.Queries;
using CalorieTracker.Domain.Entities;
using CalorieTracker.Domain.ValueObjects;
using CalorieTracker.Domain.Services;

namespace CalorieTracker.Api.Mapping
{
	/// <summary>
	/// Profil AutoMapper dla mapowania obiektów związanych z przepisami
	/// </summary>
	public class RecipeMappingProfile : Profile
	{
		public RecipeMappingProfile()
		{
			// Request -> Command mappings
			CreateMap<CreateRecipeRequest, CreateRecipeCommand>()
				.ForMember(dest => dest.CreatedByUserId, opt => opt.Ignore()); // Ustawiamy w kontrolerze

			CreateMap<CreateRecipeIngredientRequest, CreateRecipeIngredientCommand>();

			CreateMap<UpdateRecipeRequest, UpdateRecipeCommand>()
				.ForMember(dest => dest.Id, opt => opt.Ignore()) // Ustawiamy w kontrolerze
				.ForMember(dest => dest.UpdatedByUserId, opt => opt.Ignore()); // Ustawiamy w kontrolerze

			// Request -> Query mappings
			CreateMap<SearchRecipesRequest, SearchRecipesQuery>();

			// Entity -> DTO mappings
			CreateMap<Recipe, RecipeDto>()
				.ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.Ingredients))
				.ForMember(dest => dest.TotalNutrition, opt => opt.Ignore()); // Kalkulujemy osobno

			CreateMap<RecipeIngredient, RecipeIngredientDto>()
				.ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
				.ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
				.ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Product.Category.ToString()))
				.ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Product.Unit.ToString()));

			CreateMap<RecipeNutrition, RecipeNutritionDto>();

			// Entity -> Summary DTO (dla list)
			CreateMap<Recipe, RecipeSummaryDto>()
				.ForMember(dest => dest.TotalCalories, opt => opt.Ignore()); // Kalkulujemy osobno
		}
	}
}