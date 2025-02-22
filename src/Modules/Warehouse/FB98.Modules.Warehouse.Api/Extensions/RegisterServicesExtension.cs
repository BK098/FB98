using FB98.Modules.Warehouse.Application.InventoryManagement.Events;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace FB98.Modules.Warehouse.Api.Extensions
{
	public static class RegisterServicesExtension
	{
		public static IServiceCollection AddRegisterServices(this IServiceCollection services)
		{
			services.AddMassTransit(config =>
			{
				// Đăng ký Consumer
				config.AddConsumer<ProductDeletedEventHandler>();
				config.AddConsumer<ProductCreatedEventHandler>();
				config.AddConsumer<PaymentSuccessEventHandler>();
				config.AddConsumer<PaymentFailedEventHandler>();
				config.AddConsumer<OrderCreatedEventHandler>();
			});

			return services;
		}
	}
}