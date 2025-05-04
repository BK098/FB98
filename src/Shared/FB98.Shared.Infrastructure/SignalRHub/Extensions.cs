using FB98.Shared.Infrastructure.Providers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace FB98.Shared.Infrastructure.SignalRHub
{
	internal static class Extensions
	{
		public static IServiceCollection AddSignalRHub(this IServiceCollection services)
		{
			services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
			services.AddSignalR();
			return services;
		}

		public static IEndpointRouteBuilder MapSignalRHubs(this IEndpointRouteBuilder endpoints)
		{
			endpoints.MapHub<SeatHub>("/webhook/seathub");
			endpoints.MapHub<NotificationHub>("/webhook/notification");
			return endpoints;
		}
	}
}