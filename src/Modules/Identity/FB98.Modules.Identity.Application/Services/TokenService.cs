using FB98.Modules.Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FB98.Modules.Identity.Application.Services
{
	public class TokenService : ITokenService
	{
		private readonly IConfiguration _configuration;
		private readonly UserManager<AppUser> _userManager;

		public TokenService(
			IConfiguration configuration,
			UserManager<AppUser> userManager)
		{
			_configuration = configuration;
			_userManager = userManager;
		}

		public string GenerateRefreshToken()
		{
			var randomBytes = new byte[64];
			using var rng = RandomNumberGenerator.Create();
			rng.GetBytes(randomBytes);
			return Convert.ToBase64String(randomBytes);
		}

		public async Task<string> GenerateAccessToken(AppUser user)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

			var claims = new List<Claim>
			{
				new(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new(ClaimTypes.Email, user.UserName!),
				new(ClaimTypes.MobilePhone, user.PhoneNumber!),
			};

			var userRoles = await _userManager.GetRolesAsync(user);
			if (userRoles.Any())
			{
				claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));
			}

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Issuer = _configuration["Jwt:Issuer"],
				Audience = _configuration["Jwt:Audience"],
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.UtcNow.AddMinutes(15),
				SigningCredentials = creds
			};
			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}
	}
}