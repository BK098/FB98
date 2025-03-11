using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FB98.Bootstrapper.Middlewares
{
	internal class JwtMiddleware : IMiddleware
	{
		private readonly ILogger<JwtMiddleware> _logger;
		private readonly IConfiguration _configuration;

		public JwtMiddleware(ILogger<JwtMiddleware> logger,
			IConfiguration configuration)
		{
			_logger = logger;
			_configuration = configuration;
		}

		public async Task InvokeAsync(HttpContext context, RequestDelegate next)
		{
			var token = context.Request.Cookies["access_token"];
			if (!string.IsNullOrEmpty(token))
			{
				var principal = GetPrincipalFromExpiredToken(token);
				if (principal == null)
				{
					context.Response.StatusCode = 401;
				}
			}
			else
			{
				context.Response.StatusCode = 401;
			}
			await next(context);
		}
		private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
		{
			var tokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),
				ValidateIssuer = false,
				ValidateAudience = false,
				ValidateLifetime = false
			};

			var tokenHandler = new JwtSecurityTokenHandler();
			var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

			if (securityToken is not JwtSecurityToken jwtSecurityToken ||
				!jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
			{
				throw new SecurityTokenException("Invalid token");
			}
			return principal;
		}
	}
}
