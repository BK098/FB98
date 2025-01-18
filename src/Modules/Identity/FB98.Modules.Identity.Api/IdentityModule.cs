using FB98.Modules.Identity.Api.Extensions;
using FB98.Modules.Identity.Application.Data;
using FB98.Modules.Identity.Application.Entities;
using FB98.Shared.Infrastructure.Postgres;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("FB98.Bootstrapper")]
namespace FB98.Modules.Identity.Api
{

	internal static class IdentityModule
	{
		internal static string JWT_KEY = "DuAnCuaNhom7AnhEmMovieNeuBanMuonThemThongTinChiTietThiToiChiu";
		public static IServiceCollection AddIdentityModule(this IServiceCollection services)
		{
			services.AddLocalization(options => options.ResourcesPath = "Modules/Identity/Resources");

			services.AddPostgres<IdentityModuleDbContext>();
			services.AddRegisterServicesIdentity();
			services.AddIdentity<AppUser, IdentityRole<Guid>>(options =>
			{
				// Tùy chỉnh các quy tắc xác thực (nếu cần)
				options.Password.RequireDigit = true;
				options.Password.RequiredLength = 6;
				options.Password.RequireUppercase = true;
				options.Password.RequireNonAlphanumeric = true;
			})
			.AddEntityFrameworkStores<IdentityModuleDbContext>()
			.AddDefaultTokenProviders();

			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JWT_KEY)),
					ValidateIssuer = false,
					ValidateAudience = false
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
			app.UseRouting();
			app.UseAuthentication();
			app.UseAuthorization();
			return app;
		}
	}
}
