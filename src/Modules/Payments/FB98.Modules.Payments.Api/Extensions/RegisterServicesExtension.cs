using Microsoft.Extensions.DependencyInjection;

namespace FB98.Modules.Payments.Api.Extensions
{
	public static class RegisterServicesExtension
	{
		public static IServiceCollection AddRegisterServices(this IServiceCollection services)
		{
			return services;
		}
	}
}
