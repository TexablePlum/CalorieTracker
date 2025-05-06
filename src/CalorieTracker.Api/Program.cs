using CalorieTracker.Api.Mapping;
using CalorieTracker.Application.Auth.Handlers;
using CalorieTracker.Application.Auth.Interfaces;
using CalorieTracker.Domain.Entities;
using CalorieTracker.Infrastructure.Auth;
using CalorieTracker.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// DbContext + SQL Server
builder.Services.AddDbContext<AppDbContext>(opts =>
	opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(opts =>
{
	opts.Password.RequiredLength = 6;
	opts.Password.RequireNonAlphanumeric = false;
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
		IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
	};
});

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "CalorieTracker API", Version = "v1" });

	// Definicja schematu bezpiecze�stwa z referencj�
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

	// Wymaganie tego schematu dla wszystkich endpoint�w
	c.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{ bearerScheme, new string[] { } }
	});
});

// Controllers
builder.Services.AddControllers();

// Application services & handlers
builder.Services.AddScoped<RegisterUserHandler>();
builder.Services.AddScoped<LoginUserHandler>();

// JWT generator 
builder.Services.AddScoped<IJwtGenerator, JwtGenerator>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(AuthProfile));

var app = builder.Build();

// Automatyczne migracje
/*
using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	db.Database.Migrate();
}
*/

// Swagger tylko w Developmencie
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "CalorieTracker API v1");
		c.RoutePrefix = string.Empty;
	});
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
