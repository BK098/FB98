using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FB98.Bootstrapper")]
namespace FB98.Modules.Systems.Api
{
	internal static class SystemModule
	{
		public static IServiceCollection AddSystemModule(this IServiceCollection services, IConfiguration configuration)
		{
			return services;
		}

		public static IApplicationBuilder UseSystemModule(this IApplicationBuilder app)
		{
			return app;
		}
	}
}