// Plik ProfileMapping.cs - konfiguracja mapowania obiektów dla modułu profilu użytkownika.
// Odpowiada za transformację między modelami API, encjami domenowymi a DTO profilu.
using AutoMapper;
using CalorieTracker.Domain.Entities;
using CalorieTracker.Api.Models.Profile;
using CalorieTracker.Domain.Enums;

/// <summary>
/// Klasa profilu AutoMappera dla operacji związanych z profilem użytkownika.
/// Definiuje mapowania między modelami API a encjami domenowymi i obiektami DTO.
/// </summary>
public class ProfileMapping : Profile
{
	/// <summary>
	/// Inicjalizuje nową instancję profilu mapowania dla profilu użytkownika.
	/// Konfiguruje mapowania wymagane do przetwarzania operacji na profilu.
	/// </summary>
	public ProfileMapping()
	{
		/// <summary>
		/// Konwersja między List<bool> a bool[] (dwukierunkowa).
		/// Wymagana dla kompatybilności między formatami przechowywania planu posiłków.
		/// </summary>
		CreateMap<List<bool>, bool[]>().ConvertUsing(src => src.ToArray());
		CreateMap<bool[], List<bool>>().ConvertUsing(src => src.ToList());

		/// <summary>
		/// Mapowanie z encji <see cref="UserProfile"/> na model <see cref="UserProfileDto"/>.
		/// Transformuje dane profilu użytkownika na format odpowiedzi API.
		/// Konwertuje enumy Gender, ActivityLevel i Goal na ich stringowe reprezentacje.
		/// </summary>
		CreateMap<UserProfile, UserProfileDto>()
			.ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender.ToString()))
			.ForMember(dest => dest.ActivityLevel, opt => opt.MapFrom(src => src.ActivityLevel.ToString()))
			.ForMember(dest => dest.Goal, opt => opt.MapFrom(src => src.Goal.ToString()));

		/// <summary>
		/// Mapowanie z <see cref="UpdateUserProfileRequest"/> na encję <see cref="UserProfile"/>.
		/// Transformuje model żądania aktualizacji profilu na encję domenową.
		/// Obsługuje konwersję stringów na enumy z obsługą wartości null.
		/// </summary>
		CreateMap<UpdateUserProfileRequest, UserProfile>()
			.ForMember(dest => dest.Gender, opt => opt.MapFrom(src =>
				string.IsNullOrWhiteSpace(src.Gender) ? (Gender?)null : Enum.Parse<Gender>(src.Gender, true)))
			.ForMember(dest => dest.ActivityLevel, opt => opt.MapFrom(src =>
				string.IsNullOrWhiteSpace(src.ActivityLevel) ? (ActivityLevel?)null : Enum.Parse<ActivityLevel>(src.ActivityLevel, true)))
			.ForMember(dest => dest.Goal, opt => opt.MapFrom(src =>
				string.IsNullOrWhiteSpace(src.Goal) ? (GoalType?)null : Enum.Parse<GoalType>(src.Goal, true)));
	}
}