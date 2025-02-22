using Microsoft.Extensions.DependencyInjection;

namespace FB98.Shared.Infrastructure.Payments.VnPay
{
	public static class Extensions
	{
		internal static IServiceCollection AddVnPay(this IServiceCollection services)
		{
			var options = services.GetOptions<VnPayOptions>("VnPay");
			services.AddSingleton(options);
			services.AddScoped<IVnPayService, VnPayService>();

			return services;
		}
	}
}