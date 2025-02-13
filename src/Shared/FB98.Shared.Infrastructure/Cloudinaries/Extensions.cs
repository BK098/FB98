using CloudinaryDotNet;
using Microsoft.Extensions.DependencyInjection;

namespace FB98.Shared.Infrastructure.Cloudinaries
{
	public static class Extensions
	{
		internal static IServiceCollection AddCloudinary(this IServiceCollection services)
		{
			var options = services.GetOptions<CloudinaryOptions>("cloudinary");
			services.AddSingleton<ICloudinaryService, CloudinaryService>();
			services.AddSingleton(options);
#if DEBUG
			try
			{
				var account = new Account(options.CloudName, options.ApiKey, options.ApiSecret);
				var cloudinary = new Cloudinary(account);
				services.AddSingleton(cloudinary);

				var testResult = cloudinary.Api.UrlImgUp.BuildUrl("test_image.jpg");
				Console.WriteLine($"Cloudinary connected successfully: {testResult}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Cloudinary connection failed: {ex.Message}");
				throw;
			}
#endif
			return services;
		}
	}
}
