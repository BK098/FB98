using FB98.Modules.Identity.Application.Models;
using FB98.Modules.Identity.Application.Services;
using FB98.Modules.Identity.Application.Validations;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace FB98.Modules.Identity.Api.Extensions
{
	public static class RegisterServicesExtension
	{
		public static IServiceCollection AddRegisterServicesIdentity(this IServiceCollection services)
		{
			services.AddScoped<IAuthenticationService, AuthenticationService>();

			services.AddScoped<IValidator<LoginDto>, LoginDtoValidation>();
			services.AddScoped<IValidator<RegisterDto>, RegisterDtoValidation>();

			return services;
		}
	}
}
