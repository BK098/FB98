using FB98.Modules.Tickets.Api.Extensions;
using FB98.Modules.Tickets.DataAccess.Data;
using FB98.Shared.Infrastructure.Postgres;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FB98.Bootstrapper")]
namespace FB98.Modules.Tickets.Api
{
	internal static class TicketModule
	{
		public static IServiceCollection AddTicketModule(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddPostgres<TicketModuleDbContext>();
			services.AddRegisterServices();
			return services;
		}
		public static IApplicationBuilder UseTicketModule(this IApplicationBuilder app)
		{
			return app;
		}
	}
}
