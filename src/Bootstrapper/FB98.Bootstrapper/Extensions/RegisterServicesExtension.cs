using FB98.Shared.Infrastructure.Repositpries;
using FluentValidation;

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
			//Console.WriteLine($"Đang đăng ký {validatorTypes.Count} validator(s):");
			foreach (var validatorType in validatorTypes)
			{
				//Console.WriteLine($" - {validatorType.FullName}");
				var dtoType = validatorType.BaseType!.GetGenericArguments()[0];
				var validatorInterface = typeof(IValidator<>).MakeGenericType(dtoType);
				services.AddScoped(validatorInterface, validatorType);
			}

			#endregion
			#region Repository
			var repositoryTypes = assemblies.SelectMany(assembly => assembly.GetTypes())
			.Where(t => !t.IsAbstract &&
						t.BaseType != null &&
						t.BaseType.IsGenericType &&
						t.BaseType.GetGenericTypeDefinition() == typeof(BaseRepository<,>) &&
						t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRepository<>)))
			.ToList();

			foreach (var repositoryType in repositoryTypes)
			{
				var entityType = repositoryType.BaseType!.GetGenericArguments()[0];
				var implementedInterfaces = repositoryType.GetInterfaces()
					.Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRepository<>) ||
								i.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRepository<>))).ToList();

				foreach (var implementedInterface in implementedInterfaces)
				{
					services.AddScoped(implementedInterface, repositoryType);
				}

				var repositoryInterface = typeof(IRepository<>).MakeGenericType(entityType);
				services.AddScoped(repositoryInterface, repositoryType);
			}
			#endregion

			return services;
		}

	}
}
