using AutoMapper;
using CalorieTracker.Domain.Entities;
using CalorieTracker.Api.Models.Profile;
using CalorieTracker.Domain.Enums;

public class ProfileMapping : Profile
{
	public ProfileMapping()
	{
		// konwersja MealPlan
		CreateMap<List<bool>, bool[]>().ConvertUsing(src => src.ToArray());
		CreateMap<bool[], List<bool>>().ConvertUsing(src => src.ToList());

		// UserProfile -> UserProfileDto 
		CreateMap<UserProfile, UserProfileDto>()
			.ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender.ToString()))
			.ForMember(dest => dest.ActivityLevel, opt => opt.MapFrom(src => src.ActivityLevel.ToString()))
			.ForMember(dest => dest.Goal, opt => opt.MapFrom(src => src.Goal.ToString()));

		// UpdateUserProfileRequest -> UserProfile
		CreateMap<UpdateUserProfileRequest, UserProfile>()
			.ForMember(dest => dest.Gender, opt => opt.MapFrom(src =>
				string.IsNullOrWhiteSpace(src.Gender) ? (Gender?)null : Enum.Parse<Gender>(src.Gender, true)))
			.ForMember(dest => dest.ActivityLevel, opt => opt.MapFrom(src =>
				string.IsNullOrWhiteSpace(src.ActivityLevel) ? (ActivityLevel?)null : Enum.Parse<ActivityLevel>(src.ActivityLevel, true)))
			.ForMember(dest => dest.Goal, opt => opt.MapFrom(src =>
				string.IsNullOrWhiteSpace(src.Goal) ? (GoalType?)null : Enum.Parse<GoalType>(src.Goal, true)));
	}
}
