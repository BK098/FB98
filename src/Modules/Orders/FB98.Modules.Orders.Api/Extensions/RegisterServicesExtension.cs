using Microsoft.Extensions.DependencyInjection;

namespace FB98.Modules.Orders.Api.Extensions
{
	public static class RegisterServicesExtension
	{
		public static IServiceCollection AddRegisterServices(this IServiceCollection services)
		{

			//services.AddMassTransit(config =>
			//{
			//	config.AddConsumer<OrderConsumer>(); // Đăng ký consumer

			//	config.UsingRabbitMq((context, cfg) =>
			//	{
			//		cfg.Host("localhost", "/", h =>
			//		{
			//			h.Username("guest");
			//			h.Password("guest");
			//		});

			//		cfg.ReceiveEndpoint("order_created_queue", e =>
			//		{
			//			e.ConfigureConsumer<OrderConsumer>(context);
			//		});
			//	});
			//});

			return services;
		}
	}
}
