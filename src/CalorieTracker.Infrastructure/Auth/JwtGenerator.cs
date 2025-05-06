using CalorieTracker.Application.Auth.Interfaces;
using CalorieTracker.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CalorieTracker.Infrastructure.Auth
{
	public class JwtGenerator : IJwtGenerator
	{
		private readonly IConfiguration _config;
		public JwtGenerator(IConfiguration config) => _config = config;

		public string CreateToken(ApplicationUser user)
		{
			var claims = new[]
			{
				new Claim(JwtRegisteredClaimNames.Sub, user.Id),
				new Claim(JwtRegisteredClaimNames.Email, user.Email)
			};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: _config["Jwt:Issuer"],
				audience: _config["Jwt:Audience"],
				claims: claims,
				expires: DateTime.UtcNow.AddHours(3),
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
