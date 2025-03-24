using FB98.Modules.Customers.Api.Extensions;
using FB98.Modules.Customers.DataAccess.Data;
using FB98.Shared.Infrastructure.Postgres;
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
			services.AddPostgres<CustomerModuleDbContext>();
			services.AddRegisterServices();

			return services;
		}

		public static IApplicationBuilder UseCustomersModule(this IApplicationBuilder app)
		{
			return app;
		}
	}
}