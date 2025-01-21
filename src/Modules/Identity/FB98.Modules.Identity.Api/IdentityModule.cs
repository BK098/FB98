using FB98.Modules.Identity.Api.Extensions;
using FB98.Modules.Identity.Application.Share.Data;
using FB98.Modules.Identity.Application.Share.Entities;
using FB98.Shared.Infrastructure.Postgres;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("FB98.Bootstrapper")]
namespace FB98.Modules.Identity.Api
{
	internal static class IdentityModule
	{
		public static IServiceCollection AddIdentityModule(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddMemoryCache();
			services.AddPostgres<IdentityModuleDbContext>();
			services.AddRegisterServicesIdentity();
			services.AddIdentity<AppUser, IdentityRole<Guid>>(options =>
			{
				options.Password.RequireDigit = true;
				options.Password.RequiredLength = 6;
				options.Password.RequireUppercase = true;
				options.Password.RequireNonAlphanumeric = true;
			})
			.AddEntityFrameworkStores<IdentityModuleDbContext>()
			.AddDefaultTokenProviders();

			services.Configure<DataProtectionTokenProviderOptions>(options =>
			{
				options.TokenLifespan = TimeSpan.FromHours(1);
			});

			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = configuration["Jwt:Issuer"], // Giá trị phải khớp
					ValidAudience = configuration["Jwt:Audience"], // Giá trị phải khớp
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
				};
				options.Events = new JwtBearerEvents
				{
					OnAuthenticationFailed = context =>
					{
						Console.WriteLine($"Authentication failed: {context.Exception.Message}");
						return Task.CompletedTask;
					}
				};
				options.SaveToken = true;
			});
			services.AddSession(options =>
			{
				options.IdleTimeout = TimeSpan.FromMinutes(30);
				options.Cookie.HttpOnly = true;
				options.Cookie.IsEssential = true;
			});

			return services;
		}
		public static IApplicationBuilder UseIdentityModule(this IApplicationBuilder app)
		{
			using (var scope = app.ApplicationServices.CreateScope())
			{
				var services = scope.ServiceProvider;
				//SeedData.Initialize(services);
			}
			//app.UseMiddleware<TokenCookieMiddleware>();
			app.UseSession();
			app.UseAuthentication();
			app.UseAuthorization();
			return app;
		}
	}
}
