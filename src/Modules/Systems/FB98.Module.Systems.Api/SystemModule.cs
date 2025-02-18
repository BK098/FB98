using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FB98.Bootstrapper")]
namespace FB98.Module.Systems.Api
{
	internal static class SystemModule
	{
		public static IServiceCollection AddSystemModule(this IServiceCollection services, IConfiguration configuration)
		{
			return services;
		}
		public static IApplicationBuilder UseSystemModule(this IApplicationBuilder app)
		{
			using (var scope = app.ApplicationServices.CreateScope())
			{
				var services = scope.ServiceProvider;
				//SeedData
			}
			return app;
		}
	}
}
