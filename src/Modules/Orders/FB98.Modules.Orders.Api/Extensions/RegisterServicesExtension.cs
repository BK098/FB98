using FB98.Modules.Orders.Application.Abstractions;
using FB98.Modules.Orders.Application.OrderManagement.BackgroundJobs;
using FB98.Modules.Orders.Application.OrderManagement.Events;
using FB98.Modules.Orders.DataAccess.Repositories;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FB98.Modules.Orders.Api.Extensions
{
	public static class RegisterServicesExtension
	{
		public static IServiceCollection AddRegisterServices(this IServiceCollection services)
		{
			services.AddSingleton<IHostedService, OrderStatusJob>();
			services.AddScoped<IUnitOfWork, UnitOfWork>();
			services.AddMassTransit(config =>
			{
				config.AddConsumer<PaymentSuccessEventHandler>();
				config.AddConsumer<VnPayPaymentCreatedEventHandler>();

				//config.UsingRabbitMq((context, cfg) =>
				//{
				//	cfg.ReceiveEndpoint("order-module-events", e =>
				//	{
				//		e.ConfigureConsumer<PaymentSuccessEventHandler>(context);
				//		e.ConfigureConsumer<VnPayPaymentCreatedEventHandler>(context);
				//	});
				//});
			});
			//services.AddScoped<PaymentSuccessEventHandler>();
			//services.AddScoped<VnPayPaymentCreatedEventHandler>();
			return services;
		}
	}
}