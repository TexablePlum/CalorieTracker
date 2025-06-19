// Plik NutritionTrackingMappingProfile.cs - konfiguracja mapowania obiektów dla modułu nutrition tracking.
// Odpowiada za transformację między modelami DTO, komendami/zapytaniami a encjami/Value Objects domenowymi.

using AutoMapper;
using CalorieTracker.Api.Models.NutritionTracking;
using CalorieTracker.Application.NutritionTracking.Commands;
using CalorieTracker.Domain.Entities;
using CalorieTracker.Domain.Enums;
using CalorieTracker.Domain.ValueObjects;

namespace CalorieTracker.Api.Mapping
{
	/// <summary>
	/// Klasa profilu AutoMappera dla operacji związanych ze śledzeniem żywienia.
	/// Definiuje mapowania między modelami DTO a komendami, zapytaniami oraz encjami/Value Objects domenowymi.
	/// Zawiera logikę transformacji danych nutrition tracking z zachowaniem enkapsulacji domeny.
	/// </summary>
	public class NutritionTrackingMappingProfile : Profile
	{
		/// <summary>
		/// Inicjalizuje nową instancję profilu mapowania dla nutrition tracking.
		/// Konfiguruje mapowania dla:
		/// - Request DTO -> Commands (LogMealRequest -> LogMealCommand)
		/// - Domain Objects -> Response DTO (DailyNutritionProgress -> DailyNutritionProgressDto)
		/// - Entities -> DTO (MealLogEntry -> MealLogEntryDto)
		/// - Enum conversions (MealType -> string)
		/// </summary>
		public NutritionTrackingMappingProfile()
		{
			ConfigureRequestToCommandMappings();
			ConfigureDomainToResponseMappings();
			ConfigureEntityToDtoMappings();
			ConfigureValueObjectMappings();
		}

		/// <summary>
		/// Konfiguruje mapowania z Request DTO na komendy aplikacji.
		/// </summary>
		private void ConfigureRequestToCommandMappings()
		{
			CreateMap<LogMealRequest, LogMealCommand>()
				.ForMember(dest => dest.MealType, opt => opt.MapFrom(src =>
					Enum.Parse<MealType>(src.MealType, true)))
				.ForMember(dest => dest.UserId, opt => opt.Ignore());

			CreateMap<UpdateMealLogRequest, UpdateMealLogCommand>()
				.ForMember(dest => dest.MealType, opt => opt.MapFrom(src =>
					Enum.Parse<MealType>(src.MealType, true)))
				.ForMember(dest => dest.MealLogEntryId, opt => opt.Ignore())
				.ForMember(dest => dest.UserId, opt => opt.Ignore());
		}

		/// <summary>
		/// Konfiguruje mapowania z Value Objects domenowych na Response DTO.
		/// </summary>
		private void ConfigureDomainToResponseMappings()
		{
			CreateMap<DailyNutritionProgress, DailyNutritionProgressDto>()
				.ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date.ToString("yyyy-MM-dd")))
				.ForMember(dest => dest.MetabolicData, opt => opt.MapFrom(src => new MetabolicDataDto
				{
					BMR = src.Goals.BMR,
					TDEE = src.Goals.TDEE
				}))
				.ForMember(dest => dest.Goals, opt => opt.MapFrom(src => src.Goals))
				.ForMember(dest => dest.Consumed, opt => opt.MapFrom(src => src.Consumed))
				.ForMember(dest => dest.Remaining, opt => opt.MapFrom(src => src.Remaining))
				.ForMember(dest => dest.ProgressPercentages, opt => opt.MapFrom(src => src.ProgressPercentages))
				.ForMember(dest => dest.MealsToday, opt => opt.MapFrom(src => src.MealsToday));
		}

		/// <summary>
		/// Konfiguruje mapowania z encji domenowych na DTO.
		/// </summary>
		private void ConfigureEntityToDtoMappings()
		{
			CreateMap<MealLogEntry, MealLogEntryDto>()
				.ForMember(dest => dest.Date, opt => opt.MapFrom(src =>
					DateOnly.FromDateTime(src.ConsumedAt).ToString("yyyy-MM-dd")))
				.ForMember(dest => dest.MealType, opt => opt.MapFrom(src => src.MealType.ToString()))
				.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.GetDisplayName()))
				.ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.GetQuantityUnit()))
				.ForMember(dest => dest.Nutrition, opt => opt.MapFrom(src => new MealNutritionDto
				{
					Calories = src.CalculatedCalories,
					Protein = src.CalculatedProtein,
					Fat = src.CalculatedFat,
					Carbohydrates = src.CalculatedCarbohydrates,
					Fiber = src.CalculatedFiber,
					Sugar = src.CalculatedSugar,
					Sodium = src.CalculatedSodium
				}));
		}

		/// <summary>
		/// Konfiguruje mapowania z Value Objects na komponenty DTO.
		/// </summary>
		private void ConfigureValueObjectMappings()
		{
			CreateMap<DailyNutritionRequirements, NutritionGoalsDto>()
				.ForMember(dest => dest.Protein, opt => opt.MapFrom(src => new MacronutrientRangeDto
				{
					MinGrams = src.ProteinMinGrams,
					MaxGrams = src.ProteinMaxGrams
				}))
				.ForMember(dest => dest.Fat, opt => opt.MapFrom(src => new MacronutrientRangeDto
				{
					MinGrams = src.FatMinGrams,
					MaxGrams = src.FatMaxGrams
				}))
				.ForMember(dest => dest.Carbohydrates, opt => opt.MapFrom(src => new MacronutrientRangeDto
				{
					MinGrams = src.CarbohydratesMinGrams,
					MaxGrams = src.CarbohydratesMaxGrams
				}));

			CreateMap<ConsumedNutrition, ConsumedNutritionDto>();

			CreateMap<RemainingNutrition, RemainingNutritionDto>()
				.ForMember(dest => dest.Protein, opt => opt.MapFrom(src => new MacronutrientRangeDto
				{
					MinGrams = src.ProteinMin,
					MaxGrams = src.ProteinMax
				}))
				.ForMember(dest => dest.Fat, opt => opt.MapFrom(src => new MacronutrientRangeDto
				{
					MinGrams = src.FatMin,
					MaxGrams = src.FatMax
				}))
				.ForMember(dest => dest.Carbohydrates, opt => opt.MapFrom(src => new MacronutrientRangeDto
				{
					MinGrams = src.CarbohydratesMin,
					MaxGrams = src.CarbohydratesMax
				}))
				.ForMember(dest => dest.WaterLiters, opt => opt.MapFrom(src => src.Water));

			CreateMap<NutritionProgressPercentages, ProgressPercentagesDto>();

			CreateMap<MealLogSummary, MealLogSummaryDto>()
				.ForMember(dest => dest.MealType, opt => opt.MapFrom(src => src.MealType.ToString()));
		}
	}
}