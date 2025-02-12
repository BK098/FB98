using FB98.Modules.Catalog.Api.Extensions;
using FB98.Modules.Catalog.DataAccess.Data;
using FB98.Shared.Infrastructure.Postgres;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FB98.Bootstrapper")]
namespace FB98.Modules.Catalog.Api
{
	internal static class CatalogModule
	{
		public static IServiceCollection AddCatalogModule(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddPostgres<CatalogModuleDbContext>();
			services.AddRegisterServices();
			return services;
		}
		public static IApplicationBuilder UseCatalogModule(this IApplicationBuilder app)
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
