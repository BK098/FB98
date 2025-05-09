using FB98.Modules.ShoppingList.Api.Extensions;
using FB98.Modules.ShoppingList.DataAccess.Data;
using FB98.Shared.Infrastructure.Postgres;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FB98.Bootstrapper")]
namespace FB98.Modules.ShoppingList.Api
{
	internal static class ShoppingListModule
	{
		public static IServiceCollection AddShoppingListModule(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddPostgres<ShoppingListModuleDbContext>();
			services.AddRegisterServices();
			return services;
		}

		public static IApplicationBuilder UseShoppingListModule(this IApplicationBuilder app)
		{
			return app;
		}
	}
}