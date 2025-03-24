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
	internal static class PaymentModule
	{
		public static IServiceCollection AddPaymentModule(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddPostgres<PaymentModuleDbContext>();
			services.AddRegisterServices();
			return services;
		}

		public static IApplicationBuilder UsePaymentModule(this IApplicationBuilder app)
		{
			return app;
		}
	}
}
