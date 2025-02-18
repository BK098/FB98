using FB98.Modules.Orders.Api.Extensions;
using FB98.Modules.Orders.DataAccess.Data;
using FB98.Shared.Infrastructure.Postgres;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FB98.Bootstrapper")]
namespace FB98.Modules.Orders.Api
{
	internal static class OrdersModule
	{
		public static IServiceCollection AddOrdersModule(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddPostgres<OrdersModuleDbContext>();
			services.AddRegisterServices();
			return services;
		}
		public static IApplicationBuilder UseOrdersModule(this IApplicationBuilder app)
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
