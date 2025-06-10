using CalorieTracker.Domain.Entities;

namespace CalorieTracker.Application.Auth.Interfaces
{
	public interface IJwtGenerator
	{
		string CreateToken(ApplicationUser user);
	}
}
