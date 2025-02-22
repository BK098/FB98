using FB98.Modules.Payments.Api.Extensions;
using FB98.Modules.Payments.DataAccess.Data;
using FB98.Shared.Infrastructure.Postgres;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FB98.Bootstrapper")]
namespace FB98.Modules.Payments.Api
{
	internal static class PaymentsModule
	{
		public static IServiceCollection AddPaymentsModule(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddPostgres<PaymentModuleDbContext>();
			services.AddRegisterServices();
			return services;
		}

		public static IApplicationBuilder UsePaymentsModule(this IApplicationBuilder app)
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
