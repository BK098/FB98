using FB98.Modules.Identity.Application.Share.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FB98.Modules.Identity.Application.Share.Services
{
	public class TokenService: ITokenService
	{
		private readonly IConfiguration _configuration;

		public TokenService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public string GenerateJwtToken(AppUser user)
		{
			var claims = new[]
			{
				new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
				new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName!),
			};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				_configuration["Jwt:Issuer"],
				_configuration["Jwt:Audience"],
				claims,
				expires: DateTime.UtcNow.AddMinutes(60),
				signingCredentials: creds);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
		public string GenerateRefreshToken()
		{
			var randomBytes = new byte[64];
			using var rng = RandomNumberGenerator.Create();
			rng.GetBytes(randomBytes);
			return Convert.ToBase64String(randomBytes);
		}
	}
}
