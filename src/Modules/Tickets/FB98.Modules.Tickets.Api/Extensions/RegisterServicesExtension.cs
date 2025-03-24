using FB98.Modules.Tickets.Application.Abstractions;
using FB98.Modules.Tickets.Application.BookingManagement.BackgroundJobs;
using FB98.Modules.Tickets.Application.BookingManagement.Events;
using FB98.Modules.Tickets.Application.SeatManagement.BackgroundJobs;
using FB98.Modules.Tickets.DataAccess.Repositories;
using FB98.Modules.Tickets.Domain.Services;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FB98.Modules.Tickets.Api.Extensions
{
	public static class RegisterServicesExtension
	{
		public static IServiceCollection AddRegisterServices(this IServiceCollection services)
		{
			services.AddScoped<HolidayChecker>();
			services.AddSingleton<IHostedService, SeatUnlockJob>();
			services.AddSingleton<IHostedService, BookingStatusJob>();
			services.AddScoped<IUnitOfWork, UnitOfWork>();
			services.AddMassTransit(config =>
			{
				config.AddConsumer<PaymentSuccessEventHandler>();
				config.AddConsumer<VnPayPaymentCreatedEventHandler>();

				//config.UsingRabbitMq((context, cfg) =>
				//{
				//	cfg.ReceiveEndpoint("ticket-module-events", e =>
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