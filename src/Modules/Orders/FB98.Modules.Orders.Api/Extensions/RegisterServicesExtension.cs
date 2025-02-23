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
				// Đăng ký Consumer
				config.AddConsumer<PaymentSuccessEventHandler>();
			});
			return services;
		}
	}
}