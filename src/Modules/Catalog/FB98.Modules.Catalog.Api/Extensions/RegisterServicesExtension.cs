using FB98.Modules.Catalog.Application.Abstractions;
using FB98.Modules.Catalog.Application.ProductManagement.Events;
using FB98.Modules.Catalog.DataAccess.Repositories;
using FB98.Shared.Abstractions.Events.Base;
using FB98.Shared.Abstractions.Events.Products;
using Microsoft.Extensions.DependencyInjection;

namespace FB98.Modules.Catalog.Api.Extensions
{
	public static class RegisterServicesExtension
	{
		public static IServiceCollection AddRegisterServices(this IServiceCollection services)
		{
			services.AddScoped<IUnitOfWork, UnitOfWork>();
			services.AddScoped<IEventHandler<StockResponseEvent>, StockResponseEventHandler>();

			return services;
		}
	}
}
