using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FB98.Bootstrapper")]
namespace FB98.Modules.Customers.Api
{
	internal static class CustomersModule
	{
		public static IServiceCollection AddCustomersModule(this IServiceCollection services, IConfiguration configuration)
		{	
			//services.AddPostgres<CustomersModuleDbContext>();
			//services.AddRegisterServicesCustomers();
			//
			return services;
		}
		public static IApplicationBuilder UseCustomersModule(this IApplicationBuilder app)
		{
			using (var scope = app.ApplicationServices.CreateScope())
			{
				var services = scope.ServiceProvider;
				//SeedData.Initialize(services);
			}
			//app.UseMiddleware<TokenCookieMiddleware>();
			return app;
		}
	}
}
