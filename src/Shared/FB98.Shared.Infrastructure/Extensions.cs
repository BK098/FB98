using FB98.Shared.Infrastructure.Api;
using FB98.Shared.Infrastructure.Email;
using FB98.Shared.Infrastructure.Events;
using FB98.Shared.Infrastructure.Exceptions;
using FB98.Shared.Infrastructure.Localization;
using FB98.Shared.Infrastructure.Messaging;
using FB98.Shared.Infrastructure.Modules;
using FB98.Shared.Infrastructure.Postgres;
using FB98.Shared.Infrastructure.Repositpries;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FB98.Bootstrapper")]
namespace FB98.Shared.Infrastructure
{
	internal static class Extensions
	{
		public static IServiceCollection AddInfrastructure(this IServiceCollection services)
		{
			AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
			services.AddTransient<IEmailSender, EmailSender>();
			services.AddLocalization(options => options.ResourcesPath = "Shared/Resources");
			services.AddControllers()
			.ConfigureApplicationPartManager(manager =>
			{
				manager.FeatureProviders.Add(new InternalControllerFeatureProvider());
			});

			services.AddSingleton<ILocalizedMessageService, LocalizedMessageService>();
			services.AddSingleton<ErrorHandlerMiddleware>();
			services.AddPostgres();
			services.AddEvents();
			services.AddMessaging();
			services.AddModuleRequests();

			services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
			var repositoryTypes = Assembly.GetAssembly(typeof(BaseRepository<,>))
				.GetTypes()
				.Where(t => !t.IsAbstract &&
							t.BaseType != null &&
							t.BaseType.IsGenericType &&
							t.BaseType.GetGenericTypeDefinition() == typeof(BaseRepository<,>) &&
							t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRepository<>)));

			foreach (var repositoryType in repositoryTypes)
			{
				var entityType = repositoryType.BaseType!.GetGenericArguments()[0];
				var implementedInterfaces = repositoryType.GetInterfaces()
					.Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRepository<>) ||
								i.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRepository<>))).ToList();

				// Đăng ký các interface mở rộng như IProductRepository thay vì IRepository<Product>
				foreach (var implementedInterface in implementedInterfaces)
				{
					services.AddScoped(implementedInterface, repositoryType);
				}

				// Đảm bảo luôn đăng ký interface generic IRepository<TEntity>
				var repositoryInterface = typeof(IRepository<>).MakeGenericType(entityType);
				services.AddScoped(repositoryInterface, repositoryType);
			}
			return services;
		}

		//Language for localization
		private static readonly string[] optionsAction = ["en", "vi"];
		public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
		{
			app.UseMiddleware<ErrorHandlerMiddleware>();
			app.UseRequestLocalization(options =>
			{
				var supportedCultures = optionsAction; // Các ngôn ngữ hỗ trợ
				options.SetDefaultCulture("vi") // Ngôn ngữ mặc định
					   .AddSupportedCultures(supportedCultures)
					   .AddSupportedUICultures(supportedCultures);
			});
			return app;
		}

		public static T GetOptions<T>(this IServiceCollection services, string sectionName) where T : new()
		{
			using var serviceProvider = services.BuildServiceProvider();
			var configuration = serviceProvider.GetRequiredService<IConfiguration>();
			var section = configuration.GetSection(sectionName);
			var options = new T();
			section.Bind(options);

			return options;
		}
	}
}