using FB98.Modules.Shows.Api.Extensions;
using FB98.Modules.Shows.DataAccess.Data;
using FB98.Shared.Infrastructure.Postgres;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FB98.Bootstrapper")]
namespace FB98.Modules.Shows.Api
{
	internal static class ShowModule
	{
		public static IServiceCollection AddShowModule(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddPostgres<ShowModuleDbContext>();
			services.AddRegisterServices();
			return services;
		}

		public static IApplicationBuilder UseShowModule(this IApplicationBuilder app)
		{
			return app;
		}
	}
}