using FB98.Modules.Shows.Application.Abstractions;
using FB98.Modules.Shows.DataAccess.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace FB98.Modules.Shows.Api.Extensions
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