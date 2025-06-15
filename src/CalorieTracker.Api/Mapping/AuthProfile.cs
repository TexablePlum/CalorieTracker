// Plik AuthProfile.cs - konfiguracja mapowania obiektów dla modułu autentykacji.
// Odpowiada za transformację między modelami API a komendami i zapytaniami aplikacji.
using AutoMapper;
using CalorieTracker.Api.Models.Auth;
using CalorieTracker.Application.Auth.Commands;
using CalorieTracker.Application.Auth.Queries;

namespace CalorieTracker.Api.Mapping
{
	/// <summary>
	/// Klasa profilu AutoMappera dla operacji związanych z autentykacją.
	/// Definiuje mapowania między modelami żądań API a obiektami komend/zapytań warstwy aplikacji.
	/// </summary>
	public class AuthProfile : Profile
	{
		/// <summary>
		/// Inicjalizuje nową instancję profilu mapowania dla autentykacji.
		/// Konfiguruje wszystkie niezbędne mapowania w konstruktorze.
		/// </summary>
		public AuthProfile()
		{
			/// <summary>
			/// Konfiguracja mapowania z <see cref="RegisterRequest"/> na <see cref="RegisterUserCommand"/>.
			/// Transformuje model żądania rejestracji z API na komendę rejestracji użytkownika.
			/// </summary>
			CreateMap<RegisterRequest, RegisterUserCommand>();

			/// <summary>
			/// Konfiguracja mapowania z <see cref="LoginRequest"/> na <see cref="LoginUserQuery"/>.
			/// Transformuje model żądania logowania z API na zapytanie logowania użytkownika.
			/// </summary>
			CreateMap<LoginRequest, LoginUserQuery>();
		}
	}
}