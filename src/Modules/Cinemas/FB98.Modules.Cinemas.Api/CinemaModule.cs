using FB98.Modules.Cinemas.Api.Extensions;
using FB98.Modules.Cinemas.DataAccess.Data;
using FB98.Shared.Infrastructure.Postgres;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FB98.Bootstrapper")]

namespace FB98.Modules.Cinemas.Api
{
	internal static class CinemaModule
	{
		public static IServiceCollection AddCinemaModule(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddPostgres<CinemaModuleDbContext>();
			services.AddRegisterServices();
			return services;
		}
		public static IApplicationBuilder UseCinemaModule(this IApplicationBuilder app)
		{
			return app;
		}
	}
}
