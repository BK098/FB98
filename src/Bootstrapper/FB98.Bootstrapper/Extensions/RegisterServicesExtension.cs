using FB98.Bootstrapper.Middlewares;
using FB98.Shared.Infrastructure.Repositpries;
using FluentValidation;
using Refit;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace FB98.Bootstrapper.Extensions
{
	public static class RegisterServicesExtension
	{
		public static IServiceCollection AddRegisterServices(this IServiceCollection services)
		{
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();

			#region MediatR

			services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies));

			#endregion

			#region AutoMapper

			services.AddAutoMapper(assemblies);

			#endregion

			#region FluentValidation

			var validatorTypes = assemblies.SelectMany(assembly => assembly.GetTypes())
				.Where(t => t.BaseType != null &&
							t.BaseType.IsGenericType &&
							t.BaseType.GetGenericTypeDefinition() == typeof(AbstractValidator<>))
				.Where(t => t != typeof(InlineValidator<>))
				.ToList();
			foreach (var validatorType in validatorTypes)
			{
				var dtoType = validatorType.BaseType!.GetGenericArguments()[0];
				var validatorInterface = typeof(IValidator<>).MakeGenericType(dtoType);
				services.AddScoped(validatorInterface, validatorType);
			}

			#endregion

			#region Repository

			var repositoryTypes = assemblies.SelectMany(assembly => assembly.GetTypes())
				.Where(t => t is { IsAbstract: false, BaseType.IsGenericType: true } &&
							t.BaseType.GetGenericTypeDefinition() == typeof(BaseRepository<,>) &&
							t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRepository<>)))
				.ToList();

			foreach (var repositoryType in repositoryTypes)
			{
				var entityType = repositoryType.BaseType!.GetGenericArguments()[0];
				var implementedInterfaces = repositoryType.GetInterfaces()
					.Where(i => (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRepository<>)) ||
								i.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRepository<>))).ToList();

				foreach (var implementedInterface in implementedInterfaces)
				{
					services.AddScoped(implementedInterface, repositoryType);
				}

				var repositoryInterface = typeof(IRepository<>).MakeGenericType(entityType);
				services.AddScoped(repositoryInterface, repositoryType);
			}

			#endregion

			#region Refit

			var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
			var refitInterfaceType = typeof(IHttpClientFactory);
			var refitInterfaces = assemblies.SelectMany(a => a.GetTypes())
				.Where(t => t.IsInterface && t.GetMethods()
					.Any(m => m.GetCustomAttributes(typeof(GetAttribute), false).Any()))
				.ToList();
			foreach (var refitInterface in refitInterfaces)
			{
				var method = typeof(RegisterServicesExtension)
					.GetMethod(nameof(AddRefitClient), BindingFlags.NonPublic | BindingFlags.Static)?
					.MakeGenericMethod(refitInterface);

				method?.Invoke(null, [services, configuration]); // Chỉ truyền `services`, không truyền baseUrl nữa
			}

			#endregion

			services.AddSingleton<JwtMiddleware>();
			services.AddSingleton<ErrorHandlerMiddleware>();
			services.AddSingleton<RequestTimingMiddleware>();

			return services;
		}

		private static void AddRefitClient<T>(IServiceCollection services, IConfiguration configuration) where T : class
		{
			var baseUrl = Environment.GetEnvironmentVariable("API_BASE_URL");
			if (string.IsNullOrEmpty(baseUrl))
			{
				baseUrl = configuration["ApiSettings:BaseUrl"];
			}

			// Nếu vẫn không có, kiểm tra IP runtime
			if (string.IsNullOrEmpty(baseUrl))
			{
				baseUrl = GetServerIp() switch
				{
					"127.0.0.1" => "http://localhost:5097", // Local chạy HTTP
					_ => "http://18.143.76.187:5000" // Server AWS chạy HTTP
				};
			}

			services.AddRefitClient<T>()
				.ConfigureHttpClient(c =>
				{
					c.BaseAddress = new Uri(baseUrl);
				});
		}

		private static string GetServerIp()
		{
			try
			{
				var host = Dns.GetHostEntry(Dns.GetHostName());
				foreach (var ip in host.AddressList)
				{
					if (ip.AddressFamily == AddressFamily.InterNetwork) // Lấy IPv4
					{
						return ip.ToString();
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error getting server IP: {ex.Message}");
			}

			return "127.0.0.1"; // Mặc định trả về localhost nếu không lấy được IP
		}
	}
}