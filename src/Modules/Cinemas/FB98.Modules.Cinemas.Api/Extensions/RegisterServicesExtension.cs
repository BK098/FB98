using FB98.Modules.Cinemas.Application.Abstractions;
using FB98.Modules.Cinemas.DataAccess.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace FB98.Modules.Cinemas.Api.Extensions
{
	public static class RegisterServicesExtension
	{
		public static IServiceCollection AddRegisterServices(this IServiceCollection services)
		{
			services.AddScoped<IUnitOfWork, UnitOfWork>();
			return services;
		}
	}
}