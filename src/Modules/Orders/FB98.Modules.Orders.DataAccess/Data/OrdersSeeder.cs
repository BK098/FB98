using FB98.Modules.Orders.Domain.Entities;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FB98.Bootstrapper")]
namespace FB98.Modules.Orders.DataAccess.Data
{
	internal static class OrdersSeeder
	{
		public static async Task SeedDataAsync(OrdersModuleDbContext context)
		{
			if (!context.OrderStatuses.Any())
			{
				var jsonData = File.ReadAllText("SeedData/Orders/OrderStatusSeed.json");
				var entities = JsonConvert.DeserializeObject<List<OrderStatus>>(jsonData, new JsonSerializerSettings());
				if (entities != null)
				{
					await context.OrderStatuses.AddRangeAsync(entities);
					await context.SaveChangesAsync();
					await Task.CompletedTask;
				}
			}
		}
	}
}