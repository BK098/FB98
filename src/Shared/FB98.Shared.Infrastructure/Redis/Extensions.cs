using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace FB98.Shared.Infrastructure.Redis
{
	public static class Extensions
	{
		internal static IServiceCollection AddRedis(this IServiceCollection services)
		{
			var options = services.GetOptions<RedisOptions>("redis");
			services.AddSingleton(options);
			Console.WriteLine($"Redis Connection: {options.ConnectionString}");

			try
			{
				var multiplexer = ConnectionMultiplexer.Connect(options.ConnectionString);
				services.AddSingleton<IConnectionMultiplexer>(multiplexer);
				var redis = multiplexer.GetDatabase();
				redis.StringSet("test_key", "Hello Redis!");
				var value = redis.StringGet("test_key");
				Console.WriteLine($"Redis Value: {value}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{ex}");
			}

			return services;
		}
	}
}