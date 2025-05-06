using CalorieTracker.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
		public AppDbContext(DbContextOptions<AppDbContext> options)
			: base(options)
		{

		}
	}
}
