using FB98.Modules.Warehouse.Api.Extensions;
using FB98.Modules.Warehouse.DataAccess.Data;
using FB98.Shared.Infrastructure.Postgres;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FB98.Bootstrapper")]
namespace FB98.Modules.Warehouse.Api
{
	internal static class WarehouseModule
	{
		public static IServiceCollection AddWarehouseModule(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddPostgres<WarehouseModuleDbContext>();
			services.AddRegisterServices();
			return services;
		}
		public static IApplicationBuilder UseWarehouseModule(this IApplicationBuilder app)
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
