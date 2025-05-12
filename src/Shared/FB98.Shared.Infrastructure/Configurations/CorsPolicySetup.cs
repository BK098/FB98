using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FB98.Shared.Infrastructure.Configurations
{
	internal static class CorsPolicySetup
	{
		//private const string CorsPolicyName = "AllowSpecificOrigins";
		private const string CorsPolicyName = "AllowAll";

		public static void AddCustomCors(this IServiceCollection services, IConfiguration configuration)
		{
			var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>();

			services.AddCors(options =>
			{
				options.AddPolicy(CorsPolicyName, policy =>
				{
					if (allowedOrigins != null && allowedOrigins.Any())
					{
						policy.AllowAnyOrigin()
							//.WithOrigins(allowedOrigins)
							//.AllowCredentials()
							.AllowAnyHeader()
							.AllowAnyMethod();
					}
				});
			});
		}

		public static void UseCustomCors(this IApplicationBuilder app)
		{
			app.UseCors(CorsPolicyName);
		}
	}
}