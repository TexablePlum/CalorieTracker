// Plik ProfileMapping.cs - konfiguracja mapowania obiektów dla modułu profilu użytkownika.
// Odpowiada za transformację między modelami API, encjami domenowymi a DTO profilu.
using AutoMapper;
using CalorieTracker.Domain.Entities;
using CalorieTracker.Api.Models.Profile;
using CalorieTracker.Domain.Enums;

namespace CalorieTracker.Api.Mapping
{
	/// <summary>
	/// Klasa profilu AutoMappera dla operacji związanych z profilem użytkownika.
	/// Definiuje mapowania między modelami API a encjami domenowymi i obiektami DTO.
	/// Obsługuje konwersje enum-string oraz transformacje kolekcji dla planu posiłków.
	/// </summary>
	public class ProfileMapping : Profile
	{
		/// <summary>
		/// Inicjalizuje nową instancję profilu mapowania dla profilu użytkownika.
		/// Konfiguruje mapowania dla:
		/// - Konwersje List&lt;bool&gt; ↔ bool[] (plan posiłków)
		/// - UserProfile -> UserProfileDto (enum na string)
		/// - UpdateUserProfileRequest -> UserProfile (string na enum z obsługą null)
		/// </summary>
		public ProfileMapping()
		{
			ConfigureMealPlanConversions();
			ConfigureUserProfileToDto();
			ConfigureUpdateRequestToUserProfile();
		}

		/// <summary>
		/// Konfiguruje dwukierunkową konwersję między List&lt;bool&gt; a bool[].
		/// Wymagana dla kompatybilności między formatami przechowywania planu posiłków.
		/// </summary>
		private void ConfigureMealPlanConversions()
		{
			CreateMap<List<bool>, bool[]>().ConvertUsing(src => src.ToArray());
			CreateMap<bool[], List<bool>>().ConvertUsing(src => src.ToList());
		}

		/// <summary>
		/// Konfiguruje mapowanie z encji UserProfile na model UserProfileDto.
		/// Konwertuje enumy Gender, ActivityLevel i Goal na ich stringowe reprezentacje.
		/// </summary>
		private void ConfigureUserProfileToDto()
		{
			CreateMap<UserProfile, UserProfileDto>()
				.ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender.ToString()))
				.ForMember(dest => dest.ActivityLevel, opt => opt.MapFrom(src => src.ActivityLevel.ToString()))
				.ForMember(dest => dest.Goal, opt => opt.MapFrom(src => src.Goal.ToString()));
		}

		/// <summary>
		/// Konfiguruje mapowanie z UpdateUserProfileRequest na encję UserProfile.
		/// Obsługuje konwersję stringów na enumy z obsługą wartości null oraz pustych stringów.
		/// </summary>
		private void ConfigureUpdateRequestToUserProfile()
		{
			CreateMap<UpdateUserProfileRequest, UserProfile>()
				.ForMember(dest => dest.Gender, opt => opt.MapFrom(src =>
					string.IsNullOrWhiteSpace(src.Gender) ? (Gender?)null : Enum.Parse<Gender>(src.Gender, true)))
				.ForMember(dest => dest.ActivityLevel, opt => opt.MapFrom(src =>
					string.IsNullOrWhiteSpace(src.ActivityLevel) ? (ActivityLevel?)null : Enum.Parse<ActivityLevel>(src.ActivityLevel, true)))
				.ForMember(dest => dest.Goal, opt => opt.MapFrom(src =>
					string.IsNullOrWhiteSpace(src.Goal) ? (GoalType?)null : Enum.Parse<GoalType>(src.Goal, true)));
		}
	}
}