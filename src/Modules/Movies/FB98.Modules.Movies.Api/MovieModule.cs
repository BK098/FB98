using FB98.Modules.Movies.Api.Extensions;
using FB98.Modules.Movies.DataAccess.Data;
using FB98.Shared.Infrastructure.Postgres;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FB98.Bootstrapper")]
namespace FB98.Modules.Movies.Api
{
	internal static class MovieModule
	{
		public static IServiceCollection AddMovieModule(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddPostgres<MovieModuleDbContext>();
			services.AddRegisterServices();
			return services;
		}

		public static IApplicationBuilder UseMovieModule(this IApplicationBuilder app)
		{
			return app;
		}
	}
}