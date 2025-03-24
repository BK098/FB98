using FB98.Modules.Customers.Application.Abstractions;
using FB98.Modules.Customers.Application.CustomerManagement.Events;
using FB98.Modules.Customers.DataAccess.Repositories;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace FB98.Modules.Customers.Api.Extensions
{
	public static class RegisterServicesExtension
	{
		public static IServiceCollection AddRegisterServices(this IServiceCollection services)
		{
			services.AddScoped<IUnitOfWork, UnitOfWork>();
			services.AddMassTransit(config =>
			{
				config.AddConsumer<PaymentSuccessEventHandler>();
			});

			return services;
		}
	}
}