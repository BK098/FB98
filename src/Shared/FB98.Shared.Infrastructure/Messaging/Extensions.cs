using FB98.Shared.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using FB98.Shared.Abstractions.Messaging;

namespace FB98.Shared.Infrastructure.Messaging
{
	internal static class Extensions
	{
		public static IServiceCollection AddMessaging(this IServiceCollection services)
		{
			var options = services.GetOptions<MessagingOptions>("messaging");
			services.AddSingleton(options);
			services.AddSingleton<IMessageBroker, InMemoryMessageBroker>();
			services.AddSingleton<IMessageChannel, MessageChannel>();
			services.AddSingleton<IAsynchronousDispatcher, AsynchronousDispatcher>();

			if (options.UseBackgroundDispatcher)
			{
				services.AddHostedService<AsynchronousDispatcherJob>();
			}

			return services;
		}
	}
}