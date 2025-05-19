using CalorieTracker.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Api.Attributes
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class RequireCompleteProfileAttribute : Attribute, IAsyncActionFilter
	{
		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var userId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (userId is null)
			{
				context.Result = new UnauthorizedResult();
				return;
			}

			var db = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
			var profile = await db.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);

			if (profile is null || !IsComplete(profile))
			{
				context.Result = new ObjectResult(new
				{
					error = "ProfileIncomplete",
					message = "Uzupełnij swój profil przed skorzystaniem z tej funkcji."
				})
				{
					StatusCode = 403
				};
				return;
			}

			await next();
		}

		private bool IsComplete(Domain.Entities.UserProfile p)
		{
			return p.Age is not null &&
				   p.Gender is not null &&
				   p.HeightCm is not null &&
				   p.WeightKg is not null &&
				   p.TargetWeightKg is not null &&
				   p.WeeklyGoalChangeKg is not null &&
				   p.ActivityLevel is not null &&
				   p.Goal is not null &&
				   p.MealPlan is not null &&
				   p.MealPlan.Length == 6 &&
				   p.MealPlan.Any(x => x); // przynajmniej jeden posiłek
		}
	}
}
