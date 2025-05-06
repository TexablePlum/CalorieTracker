using CalorieTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalorieTracker.Application.Auth.Interfaces
{
	public interface IJwtGenerator
	{
		string CreateToken(ApplicationUser user);
	}
}
