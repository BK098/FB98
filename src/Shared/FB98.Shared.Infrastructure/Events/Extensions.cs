using FB98.Shared.Abstractions.Events.Base;
using Microsoft.Extensions.DependencyInjection;

namespace FB98.Shared.Infrastructure.Events
{
	internal static class Extensions
	{
		public static IServiceCollection AddEvents(this IServiceCollection services)
		{
			services.AddSingleton<IEventDispatcher, EventDispatcher>();

			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			services.Scan(s => s.FromAssemblies(assemblies)
				.AddClasses(c => c.AssignableTo(typeof(IEventHandler<>)))
				.AsImplementedInterfaces()
				.WithScopedLifetime());

			return services;
		}
	}
}