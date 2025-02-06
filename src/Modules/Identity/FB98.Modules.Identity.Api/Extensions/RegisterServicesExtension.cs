using FB98.Modules.Identity.Application;
using FB98.Modules.Identity.Application.Abtractions;
using FB98.Modules.Identity.Application.Authentication.ForgotPassword;
using FB98.Modules.Identity.Application.Authentication.Login;
using FB98.Modules.Identity.Application.Authentication.Register;
using FB98.Modules.Identity.Application.Authentication.ResetPassword;
using FB98.Modules.Identity.Application.ProfileManagement.ChangePassword;
using FB98.Modules.Identity.Application.Services;
using FB98.Modules.Identity.DataAccess.Repositories;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace FB98.Modules.Identity.Api.Extensions
{
	public static class RegisterServicesExtension
	{
		public static IServiceCollection AddRegisterServicesIdentity(this IServiceCollection services)
		{
			services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationAssemblyMarker).GetTypeInfo().Assembly));

			services.AddScoped<ITokenService, TokenService>();
			services.AddScoped<ITokenStoreRepository, TokenStoreRepository>();

			services.AddScoped<IValidator<LoginDto>, LoginValidation>();
			services.AddScoped<IValidator<RegisterDto>, RegisterDtoValidation>();
			services.AddScoped<IValidator<ForgotPasswordDto>, ForgotPasswordDtoValidation>();
			services.AddScoped<IValidator<ResetPasswordDto>, ResetPasswordDtoValidation>();
			services.AddScoped<IValidator<ChangePasswordDto>, ChangePasswordDtoValidation>();

			return services;
		}
	}
}
