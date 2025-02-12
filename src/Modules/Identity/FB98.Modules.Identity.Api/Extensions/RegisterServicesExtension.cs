using FB98.Modules.Identity.Application.Abtractions;
using FB98.Modules.Identity.Application.Services;
using FB98.Modules.Identity.DataAccess.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace FB98.Modules.Identity.Api.Extensions
{
	public static class RegisterServicesExtension
	{
		public static IServiceCollection AddRegisterServicesIdentity(this IServiceCollection services)
		{

			services.AddScoped<ITokenService, TokenService>();
			services.AddScoped<ITokenStoreRepository, TokenStoreRepository>();

			return services;
		}
	}
}
