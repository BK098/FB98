using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace FB98.Shared.Infrastructure.SignalRHub
{
	internal static class Extensions
	{
		public static IServiceCollection AddSignalRHub(this IServiceCollection services)
		{
			services.AddSignalR();
			return services;
		}


		public static IEndpointRouteBuilder MapSignalRHubs(this IEndpointRouteBuilder endpoints)
		{
			endpoints.MapHub<SeatHub>("/webhook/seathub"); // hoặc /seathub
			return endpoints;
		}
	}
}