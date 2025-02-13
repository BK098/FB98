using FB98.Shared.Infrastructure.Api;
using FB98.Shared.Infrastructure.Cloudinaries;
using FB98.Shared.Infrastructure.Email;
using FB98.Shared.Infrastructure.Events;
using FB98.Shared.Infrastructure.Exceptions;
using FB98.Shared.Infrastructure.Localization;
using FB98.Shared.Infrastructure.Messaging;
using FB98.Shared.Infrastructure.Modules;
using FB98.Shared.Infrastructure.Postgres;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
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
			services.AddCloudinary();

			return services;
		}

		//Language for localization
		private static readonly string[] optionsAction = { "en", "vi" };
		public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
		{
			app.UseMiddleware<ErrorHandlerMiddleware>();

			var supportedCultures = new[]
			{
				new CultureInfo("en"),
				new CultureInfo("vi")
			};

			var localizationOptions = new RequestLocalizationOptions
			{
				DefaultRequestCulture = new RequestCulture("en"),
				SupportedCultures = supportedCultures,
				SupportedUICultures = supportedCultures,
				RequestCultureProviders = new List<IRequestCultureProvider>
				{
					new QueryStringRequestCultureProvider(),  // Hỗ trợ đổi qua query string: ?culture=vi
					new CookieRequestCultureProvider(),       // Lưu ngôn ngữ vào cookie
					new AcceptLanguageHeaderRequestCultureProvider() // Nếu không có query hoặc cookie, lấy từ header
				}
			};

			app.UseRequestLocalization(localizationOptions);
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