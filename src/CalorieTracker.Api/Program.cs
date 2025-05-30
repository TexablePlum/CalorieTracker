using CalorieTracker.Api.Mapping;
using CalorieTracker.Api.Validation;
using CalorieTracker.Application.Auth.Handlers;
using CalorieTracker.Application.Auth.Interfaces;
using CalorieTracker.Application.Interfaces;
using CalorieTracker.Domain.Entities;
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

// Polityka CORS dla frontu webowego
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowFrontendWeb", policy =>
	{
		policy.WithOrigins(
				"http://127.0.0.1:5500",
				"http://localhost:5500"
			)
			.AllowAnyHeader()
			.AllowAnyMethod();
	});
});

// DbContext + SQL Server
builder.Services.AddDbContext<AppDbContext>(opts =>
	opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>()); // Abstrakcja AppDbContext aby wyeliminowaæ zale¿noœæ handlerów od modu³u infrastructure

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(opts =>
{
	// U¯YTKOWNIK
	opts.User.RequireUniqueEmail = true; // unikalny mail

	// HAS£O
	opts.Password.RequiredLength = 8;
	opts.Password.RequireDigit = true;
	opts.Password.RequireUppercase = true;
	opts.Password.RequireLowercase = true;
	opts.Password.RequireNonAlphanumeric = true;

	// LOCKOUT
	opts.Lockout.MaxFailedAccessAttempts = 5; // liczba b³edów
	opts.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15); // - minut przerwy
	opts.Lockout.AllowedForNewUsers = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"]!;
var keyBytes = Encoding.UTF8.GetBytes(jwtKey);
builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(opts =>
{
	opts.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidIssuer = builder.Configuration["Jwt:Issuer"],
		ValidAudience = builder.Configuration["Jwt:Audience"],
		IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
		ClockSkew = TimeSpan.Zero // TODO: Do usuniêcia !!!
	};
});

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "CalorieTracker API", Version = "v1" });

	// Definicja schematu bezpieczeñstwa z referencj¹
	var bearerScheme = new OpenApiSecurityScheme
	{
		Name = "Authorization",
		Description = "Wpisz token",
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

	// Wymaganie tego schematu dla wszystkich endpointów
	c.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{ bearerScheme, new string[] { } }
	});
});

// Controllers
builder.Services.AddControllers();

// FluentValidation – automatyczna walidacja modeli
builder.Services.AddFluentValidationAutoValidation()
				.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

// Application services & handlers
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

// JWT generator 
builder.Services.AddScoped<IJwtGenerator, JwtGenerator>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(AuthProfile));
builder.Services.AddAutoMapper(typeof(ProfileMapping));
builder.Services.AddAutoMapper(typeof(ProductMappingProfile));

// Email
builder.Services.Configure<EmailSettings>(
	builder.Configuration.GetSection("Email"));

builder.Services.AddTransient<IEmailSender, EmailSender>();


var app = builder.Build();

// Automatyczne migracje
/*
using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	db.Database.Migrate();
}
*/

// Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
	c.SwaggerEndpoint("/swagger/v1/swagger.json", "CalorieTracker API v1");
	c.RoutePrefix = string.Empty;
});

app.UseCors("AllowFrontendWeb");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
