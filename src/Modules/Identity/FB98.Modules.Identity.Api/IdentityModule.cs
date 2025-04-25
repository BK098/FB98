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
			services.AddMemoryCache();
			services.AddPostgres<IdentityModuleDbContext>();
			services.AddRegisterServicesIdentity();
			services.AddIdentity<AppUser, IdentityRole<Guid>>(options =>
				{
					options.Password.RequireDigit = true;
					options.Password.RequiredLength = 8;
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
						ValidIssuer = configuration["Jwt:Issuer"],
						ValidAudience = configuration["Jwt:Audience"],
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
							var accessToken = context.Request.Query["access_token"];
							var path = context.HttpContext.Request.Path;

							if (!string.IsNullOrEmpty(accessToken) &&
								(path.StartsWithSegments("/webhook/notification") || path.StartsWithSegments("/webhook/seathub")))
							{
								context.Token = accessToken;
							}
							else
							{
								// 👇 Nếu gọi API thông thường thì lấy từ cookie
								var cookieToken = context.Request.Cookies["access_token"];
								if (!string.IsNullOrEmpty(cookieToken))
								{
									context.Token = cookieToken;
								}
							}
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
			//app.UseMiddleware<TokenCookieMiddleware>();
			app.UseSession();
			app.UseAuthentication();
			app.UseAuthorization();
			return app;
		}
	}
}