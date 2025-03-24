using FB98.Shared.Infrastructure.Api;
using FB98.Shared.Infrastructure.Cloudinaries;
using FB98.Shared.Infrastructure.Email;
using FB98.Shared.Infrastructure.Localization;
using FB98.Shared.Infrastructure.Middlewares;
using FB98.Shared.Infrastructure.Payments.VnPay;
using FB98.Shared.Infrastructure.Postgres;
using FB98.Shared.Infrastructure.RabbitMq;
using FB98.Shared.Infrastructure.Redis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
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
			services.AddControllers()
				.AddNewtonsoftJson(options =>
				{
					options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
				})
				.ConfigureApplicationPartManager(manager =>
				{
					manager.FeatureProviders.Add(new InternalControllerFeatureProvider());
				});
			services.AddDistributedMemoryCache();
			services.AddEndpointsApiExplorer();
			services.AddLocalization(options => options.ResourcesPath = "Shared/Resources");
			services.AddRabbitMq();
			services.AddTransient<IEmailSender, EmailSender>();
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.AddSingleton<ILocalizedMessageService, LocalizedMessageService>();
			services.AddSingleton<ErrorHandlerMiddleware>();
			services.AddSingleton<RequestTimingMiddleware>();
			services.AddPostgres();
			services.AddCloudinary();
			services.AddVnPay();
			services.AddRedis();
			services.AddSignalR();
			return services;
		}

		public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
		{
			app.UseMiddleware<ErrorHandlerMiddleware>();
			app.UseMiddleware<RequestTimingMiddleware>();
			//Language for localization
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
					new QueryStringRequestCultureProvider(), // Hỗ trợ đổi qua query string: ?culture=vi
					new CookieRequestCultureProvider(), // Lưu ngôn ngữ vào cookie
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