using AutoMapper;
using CalorieTracker.Api.Models.Auth;
using CalorieTracker.Application.Auth.Commands;
using CalorieTracker.Application.Auth.Queries;

namespace CalorieTracker.Api.Mapping
{
	public class AuthProfile : Profile
	{
		public AuthProfile()
		{
			CreateMap<RegisterRequest, RegisterUserCommand>();
			CreateMap<LoginRequest, LoginUserQuery>();
		}
	}
}
