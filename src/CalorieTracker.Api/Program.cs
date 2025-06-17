// Plik `Program.cs` - g³ówny punkt wejœcia do aplikacji.
// Odpowiedzialny za skonfigurowanie i uruchomienie hosta aplikacji webowej,
// w tym za rejestracjê wszystkich serwisów w kontenerze wstrzykiwania zale¿noœci (DI),
// oraz za konfiguracjê potoku przetwarzania ¿¹dañ HTTP (middleware).

using CalorieTracker.Api.Mapping;
using CalorieTracker.Api.Validation;
using CalorieTracker.Application.Auth.Handlers;
using CalorieTracker.Application.Auth.Interfaces;
using CalorieTracker.Application.Interfaces;
using CalorieTracker.Application.Recipes.Handlers;
using CalorieTracker.Application.WeightMeasurements.Handlers;
using CalorieTracker.Application.WeightMeasurements.Services;
using CalorieTracker.Domain.Entities;
using CalorieTracker.Domain.Services;
using CalorieTracker.Infrastructure.Auth;
using CalorieTracker.Infrastructure.Data;
using CalorieTracker.Infrastructure.Email;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Konfiguracja polityki Cross-Origin Resource Sharing (CORS) dla aplikacji klienckiej.
// TODO: Atrapa na potrzeby zespo³u frontu webowego DO MODYFIKACJI w przysz³oœci !
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowFrontendWeb", policy =>
	{
		// Zezwolenie na ¿¹dania z okreœlonych Ÿróde³.
		policy.WithOrigins(
				"http://127.0.0.1:5500",
				"http://localhost:5500",
				"http://127.0.0.1:5501",
				"http://localhost:5501"
			)
			.AllowAnyHeader() // Zezwolenie na dowolne nag³ówki.
			.AllowAnyMethod(); // Zezwolenie na dowolne metody HTTP.
	});
});

// Rejestracja kontekstu bazy danych (AppDbContext) z u¿yciem SQL Server.
builder.Services.AddDbContext<AppDbContext>(opts =>
	// U¿ycie connection stringa z pliku konfiguracyjnego.
	opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Rejestracja abstrakcji IAppDbContext w celu odizolowania warstwy Application od implementacji w Infrastructure.
builder.Services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

// Dodanie i konfiguracja systemu to¿samoœci (Identity).
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(opts =>
{
	// Konfiguracja wymagañ dotycz¹cych u¿ytkownika.
	opts.User.RequireUniqueEmail = true;

	// Konfiguracja wymagañ dotycz¹cych has³a.
	opts.Password.RequiredLength = 8;
	opts.Password.RequireDigit = true;
	opts.Password.RequireUppercase = true;
	opts.Password.RequireLowercase = true;
	opts.Password.RequireNonAlphanumeric = true;

	// Konfiguracja mechanizmu blokady konta po nieudanych próbach logowania.
	opts.Lockout.MaxFailedAccessAttempts = 5;
	opts.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
	opts.Lockout.AllowedForNewUsers = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders(); // Dodanie domyœlnych dostawców tokenów (np. do resetu has³a).

// Dodanie i konfiguracja uwierzytelniania za pomoc¹ tokenów JWT.
var jwtKey = builder.Configuration["Jwt:Key"]!;
var keyBytes = Encoding.UTF8.GetBytes(jwtKey);
builder.Services.AddAuthentication(options =>
{
	// Ustawienie domyœlnych schematów uwierzytelniania.
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(opts =>
{
	// Konfiguracja parametrów walidacji tokenu JWT.
	opts.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true, // Walidacja wystawcy.
		ValidateAudience = true, // Walidacja odbiorcy.
		ValidateLifetime = true, // Walidacja czasu ¿ycia tokenu.
		ValidIssuer = builder.Configuration["Jwt:Issuer"],
		ValidAudience = builder.Configuration["Jwt:Audience"],
		IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
		ClockSkew = TimeSpan.Zero // Usuniêcie domyœlnej tolerancji czasowej przy walidacji wa¿noœci tokenu.
	};
});

// Dodanie i konfiguracja generatora dokumentacji Swagger/OpenAPI.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "CalorieTracker API", Version = "v1" });

	// Zdefiniowanie schematu bezpieczeñstwa "Bearer" dla autoryzacji JWT.
	var bearerScheme = new OpenApiSecurityScheme
	{
		Name = "Authorization",
		Description = "Wprowadzenie tokenu JWT w formacie 'Bearer {token}'",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.Http,
		Scheme = "bearer",
		BearerFormat = "JWT",
		Reference = new OpenApiReference
		{
			Type = ReferenceType.SecurityScheme,
			Id = "Bearer"
		}
	};
	c.AddSecurityDefinition("Bearer", bearerScheme);

	// Wymuszenie stosowania schematu bezpieczeñstwa "Bearer" dla wszystkich endpointów.
	c.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{ bearerScheme, new string[] { } }
	});
});

// Dodanie serwisów kontrolerów MVC.
builder.Services.AddControllers();

// Dodanie i konfiguracja FluentValidation do automatycznej walidacji ¿¹dañ.
builder.Services.AddFluentValidationAutoValidation()
				// Rejestracja walidatorów z okreœlonych assembly.
				.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>()
				.AddValidatorsFromAssemblyContaining<CreateRecipeRequestValidator>()
				.AddValidatorsFromAssemblyContaining<CreateWeightMeasurementRequestValidator>();

// Rejestracja w kontenerze DI wszystkich serwisów i handlerów z warstwy Application oraz Domain.
builder.Services.AddScoped<RegisterUserHandler>();
builder.Services.AddScoped<GenerateEmailConfirmationHandler>();
builder.Services.AddScoped<LoginUserHandler>();
builder.Services.AddScoped<GenerateRefreshTokenHandler>();
builder.Services.AddScoped<UseRefreshTokenHandler>();
builder.Services.AddScoped<LogoutHandler>();
builder.Services.AddScoped<GeneratePasswordResetHandler>();
builder.Services.AddScoped<CreateProductHandler>();
builder.Services.AddScoped<UpdateProductHandler>();
builder.Services.AddScoped<DeleteProductHandler>();
builder.Services.AddScoped<SearchProductsHandler>();
builder.Services.AddScoped<GetProductByIdHandler>();
builder.Services.AddScoped<GetProductByBarcodeHandler>();
builder.Services.AddScoped<GetUserProductsHandler>();
builder.Services.AddScoped<CreateRecipeHandler>();
builder.Services.AddScoped<UpdateRecipeHandler>();
builder.Services.AddScoped<DeleteRecipeHandler>();
builder.Services.AddScoped<GetRecipeDetailsHandler>();
builder.Services.AddScoped<SearchRecipesHandler>();
builder.Services.AddScoped<GetUserRecipesHandler>();
builder.Services.AddScoped<GetAllRecipesHandler>();
builder.Services.AddScoped<RecipeNutritionCalculator>();
builder.Services.AddScoped<CreateWeightMeasurementHandler>();
builder.Services.AddScoped<UpdateWeightMeasurementHandler>();
builder.Services.AddScoped<DeleteWeightMeasurementHandler>();
builder.Services.AddScoped<GetUserWeightMeasurementsHandler>();
builder.Services.AddScoped<GetLatestWeightMeasurementHandler>();
builder.Services.AddScoped<GetWeightMeasurementDetailsHandler>();
builder.Services.AddScoped<WeightAnalysisService>();
builder.Services.AddScoped<WeightMeasurementRecalculationService>();

// Rejestracja generatora tokenów JWT.
builder.Services.AddScoped<IJwtGenerator, JwtGenerator>();

// Rejestracja profili mapowania AutoMapper.
builder.Services.AddAutoMapper(typeof(AuthProfile));
builder.Services.AddAutoMapper(typeof(ProfileMapping));
builder.Services.AddAutoMapper(typeof(ProductMappingProfile));
builder.Services.AddAutoMapper(typeof(RecipeMappingProfile));
builder.Services.AddAutoMapper(typeof(WeightMeasurementMappingProfile));

// Konfiguracja ustawieñ poczty email.
builder.Services.Configure<EmailSettings>(
	builder.Configuration.GetSection("Email"));

// Rejestracja serwisu do wysy³ania emaili.
builder.Services.AddTransient<IEmailSender, EmailSender>();


var app = builder.Build();

// Zastosowanie automatycznych migracji bazy danych przy starcie aplikacji.
/*
using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	db.Database.Migrate();
}
*/

// W³¹czenie middleware Swagger i Swagger UI.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
	// Ustawienie endpointu dla definicji API.
	c.SwaggerEndpoint("/swagger/v1/swagger.json", "CalorieTracker API v1");
	// Ustawienie interfejsu Swaggera jako strony g³ównej aplikacji.
	c.RoutePrefix = string.Empty;
});

// W³¹czenie polityki CORS.
app.UseCors("AllowFrontendWeb");

// W³¹czenie middleware uwierzytelniania i autoryzacji.
app.UseAuthentication();
app.UseAuthorization();

// Zmapowanie tras do kontrolerów.
app.MapControllers();

// Uruchomienie aplikacji.
app.Run();