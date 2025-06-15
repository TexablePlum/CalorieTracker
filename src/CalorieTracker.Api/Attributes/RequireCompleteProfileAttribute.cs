// Plik RequireCompleteProfileAttribute.cs - atrybut autoryzacji wymagający kompletnego profilu użytkownika.
// Implementuje filtr akcji sprawdzający czy profil użytkownika zawiera wszystkie wymagane dane przed wykonaniem akcji kontrolera.

using CalorieTracker.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Api.Attributes
{
	/// <summary>
	/// Atrybut autoryzacji wymagający kompletnego profilu użytkownika przed dostępem do zasobów.
	/// Implementuje filtr akcji sprawdzający wszystkie wymagane pola profilu użytkownika.
	/// Automatycznie blokuje dostęp do funkcjonalności wymagających pełnych danych dietetycznych.
	/// Może być stosowany na poziomie kontrolera lub pojedynczych akcji.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class RequireCompleteProfileAttribute : Attribute, IAsyncActionFilter
	{
		/// <summary>
		/// Główna metoda filtra wykonywana przed każdą akcją kontrolera.
		/// Weryfikuje autoryzację użytkownika i kompletność jego profilu dietetycznego.
		/// Blokuje wykonanie akcji jeśli profil nie zawiera wszystkich wymaganych danych.
		/// </summary>
		/// <param name="context">Kontekst wykonania akcji zawierający informacje o żądaniu HTTP.</param>
		/// <param name="next">Delegat do wykonania następnej akcji w pipeline'ie żądania.</param>
		/// <returns>Task reprezentujący operację asynchroniczną filtra.</returns>
		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			// Ekstraktuje ID użytkownika z claims tokenu JWT
			var userId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (userId is null)
			{
				// Zwraca 401 gdy użytkownik nie jest zalogowany
				context.Result = new UnauthorizedResult();
				return;
			}

			// Pobiera kontekst bazy danych z kontenera DI
			var db = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();

			// Wyszukuje profil użytkownika w bazie danych
			var profile = await db.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);

			// Sprawdza czy profil istnieje i jest kompletny
			if (profile is null || !IsComplete(profile))
			{
				// Zwraca 403 Forbidden z opisowym komunikatem błędu
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

			// Kontynuuje wykonanie akcji gdy profil jest kompletny
			await next();
		}

		/// <summary>
		/// Sprawdza czy profil użytkownika zawiera wszystkie wymagane dane.
		/// Weryfikuje pola osobiste, cele dietetyczne oraz plan posiłków.
		/// Implementuje logikę biznesową określającą minimalny zestaw danych wymaganych do korzystania z aplikacji.
		/// </summary>
		/// <param name="p">Profil użytkownika do sprawdzenia kompletności.</param>
		/// <returns>
		/// true - gdy profil zawiera wszystkie wymagane dane i jest kompletny.
		/// false - gdy brakuje któregokolwiek z wymaganych pól lub plan posiłków jest pusty.
		/// </returns>
		private bool IsComplete(Domain.Entities.UserProfile p)
		{
			return p.Age is not null &&                 // Wiek użytkownika
				p.Gender is not null &&                 // Płeć (wymagana do kalkulacji BMR)
				p.HeightCm is not null &&               // Wzrost w centymetrach
				p.WeightKg is not null &&               // Aktualna masa ciała
				p.TargetWeightKg is not null &&         // Docelowa masa ciała
				p.WeeklyGoalChangeKg is not null &&     // Tygodniowa zmiana masy (cel)
				p.ActivityLevel is not null &&          // Poziom aktywności fizycznej
				p.Goal is not null &&                   // Główny cel (redukcja/utrzymanie/przyrost)
				p.MealPlan is not null &&               // Plan posiłków
				p.MealPlan.Length == 6 &&               // Dokładnie 6 posiłków w planie
				p.MealPlan.Any(x => x);                 // Przynajmniej jeden posiłek wybrany
		}
	}
}