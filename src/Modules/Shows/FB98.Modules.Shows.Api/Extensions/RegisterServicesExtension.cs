using FB98.Modules.Shows.Application.Abstractions;
using FB98.Modules.Shows.Application.ShowManagement.BackgroundJobs;
using FB98.Modules.Shows.DataAccess.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FB98.Modules.Shows.Api.Extensions
{
	public static class RegisterServicesExtension
	{
		public static IServiceCollection AddRegisterServices(this IServiceCollection services)
		{
			services.AddSingleton<IHostedService, ShowStatusJob>();
			services.AddScoped<IUnitOfWork, UnitOfWork>();
			return services;
		}
	}
}