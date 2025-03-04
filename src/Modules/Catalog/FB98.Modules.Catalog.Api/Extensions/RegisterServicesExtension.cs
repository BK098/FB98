using FB98.Modules.Catalog.Application.Abstractions;
using FB98.Modules.Catalog.Application.DiscountManagement.Events;
using FB98.Modules.Catalog.DataAccess.Repositories;
using FB98.Modules.Catalog.Domain.Services;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace FB98.Modules.Catalog.Api.Extensions
{
	public static class RegisterServicesExtension
	{
		public static IServiceCollection AddRegisterServices(this IServiceCollection services)
		{
			services.AddScoped<ProductDiscountDomainService>();
			services.AddScoped<IUnitOfWork, UnitOfWork>();
			services.AddMassTransit(config =>
			{
				config.AddConsumer<OrderCreatedEventHandler>();
			});
			return services;
		}
	}
}