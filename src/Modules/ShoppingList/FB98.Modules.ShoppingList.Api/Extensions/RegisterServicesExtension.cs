using FB98.Modules.ShoppingList.Application.Abstractions;
using FB98.Modules.ShoppingList.DataAccess.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace FB98.Modules.ShoppingList.Api.Extensions
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