using FB98.Modules.Identity.Api.Extensions;
using FB98.Modules.Identity.DataAccess.Data;
using FB98.Modules.Identity.Domain.Entities;
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
			services.AddHttpContextAccessor();
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
				options.RequireHttpsMetadata = false;
				options.SaveToken = true;
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
						Console.WriteLine($@"Authentication failed: {context.Exception.Message}");
						return Task.CompletedTask;
					},
					OnMessageReceived = context =>
					{
						// Thử lấy token trong Cookie "access_token"
						var accessToken = context.Request.Cookies["access_token"];
						if (!string.IsNullOrEmpty(accessToken))
						{
							context.Token = accessToken;
						}
						return Task.CompletedTask;
					}
				};
				options.SaveToken = true;
			});
			services.AddSession(options =>
			{
				options.IdleTimeout = TimeSpan.FromMinutes(30);
				options.Cookie.HttpOnly = false;
				options.Cookie.IsEssential = true;
			});

			return services;
		}
		public static IApplicationBuilder UseIdentityModule(this IApplicationBuilder app)
		{
			//app.UseMiddleware<TokenCookieMiddleware>();
			app.UseSession();
			app.UseAuthentication();
			app.UseAuthorization();
			return app;
		}
	}
}
