using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace FB98.Shared.Infrastructure.Configurations
{
	internal static class SwaggerSetup
	{
		public static void AddCustomSwagger(this IServiceCollection services)
		{
			services.AddFluentValidationRulesToSwagger();
			services.AddSwaggerGen(opt =>
			{
				opt.EnableAnnotations();
				opt.UseInlineDefinitionsForEnums();
				opt.SwaggerDoc("v1", new OpenApiInfo
				{
					Title = "Cinema FB98 Cinema",
					Version = "v1",
					Description = "API Documentation của hệ thống quản lý phòng vé",
					Contact = new OpenApiContact
					{
						Name = "Khang",
						Email = "buikhang122004@gmail.com",
					}
				});

				// Cấu hình bảo mật bằng JWT
				opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					Name = "Authorization",
					Type = SecuritySchemeType.Http,
					In = ParameterLocation.Header,
					Scheme = "bearer",
					BearerFormat = "JWT",
					Description = "Nhập Bearer Token vào đây, hứa không làm gì!"
				});

				opt.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Id = "Bearer",
								Type = ReferenceType.SecurityScheme
							}
						},
						Array.Empty<string>()
					}
				});
			});
		}

		public static void UseCustomSwagger(this IApplicationBuilder app)
		{
			app.UseSwagger();
			app.UseSwaggerUI(opt =>
			{
				opt.EnableFilter();
				opt.SwaggerEndpoint("/swagger/v1/swagger.json", "Cinema FB98 Cinema");
			});
		}
	}
}