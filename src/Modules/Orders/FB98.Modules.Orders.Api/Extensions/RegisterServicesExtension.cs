using FB98.Modules.Orders.Application.OrderManagement.BackgroundJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FB98.Modules.Orders.Api.Extensions
{
	public static class RegisterServicesExtension
	{
		public static IServiceCollection AddRegisterServices(this IServiceCollection services)
		{
			services.AddSingleton<IHostedService, OrderStatusJob>();

			return services;
		}
	}
}