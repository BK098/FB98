using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace FB98.Shared.Infrastructure.RabbitMq
{
	public static class Extensions
	{
		public static IServiceCollection AddRabbitMq(this IServiceCollection services)
		{

			var options = services.GetOptions<RabbitMqOptions>("rabbitMq");
			services.AddSingleton(options);

			services.Configure<MassTransitHostOptions>(opt =>
			{
				opt.WaitUntilStarted = true; // Đảm bảo MassTransit khởi động cùng hệ thống
			});

			services.AddMassTransit(opt =>
			{
				opt.SetKebabCaseEndpointNameFormatter();
				opt.UsingRabbitMq((context, cfg) =>
				{
					cfg.Host(options.HostName, options.VirtualHost, h =>
					{
						h.Username(options.UserName);
						h.Password(options.Password);
					});

					cfg.ConfigureEndpoints(context);
				});
			});
			return services;
		}
	}
}
