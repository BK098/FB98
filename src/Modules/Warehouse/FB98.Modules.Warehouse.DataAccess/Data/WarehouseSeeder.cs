using FB98.Modules.Warehouse.Domain.Entities;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FB98.Bootstrapper")]
namespace FB98.Modules.Warehouse.DataAccess.Data
{
	internal class WarehouseSeeder
	{
		public static async Task SeedDataAsync(WarehouseModuleDbContext context)
		{
			if (!context.Inventories.Any())
			{
				var jsonData = File.ReadAllText("SeedData/Warehouse/InventorySeed.json");
				var entities = JsonConvert.DeserializeObject<List<Inventory>>(jsonData, new JsonSerializerSettings());
				if (entities != null)
				{
					await context.Inventories.AddRangeAsync(entities);
					await context.SaveChangesAsync();
					await Task.CompletedTask;
				}
			}
		}
	}
}
