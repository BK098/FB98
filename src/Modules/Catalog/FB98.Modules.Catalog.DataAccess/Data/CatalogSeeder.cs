using FB98.Modules.Catalog.Domain.Entities;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FB98.Bootstrapper")]

namespace FB98.Modules.Catalog.DataAccess.Data
{
	internal static class CatalogSeeder
	{
		public static async Task SeedDataAsync(CatalogModuleDbContext context)
		{
			if (!context.Categories.Any())
			{
				var jsonData = await File.ReadAllTextAsync("SeedData/Catalog/CategorySeed.json");
				var entities = JsonConvert.DeserializeObject<List<Category>>(jsonData, new JsonSerializerSettings());
				if (entities != null)
				{
					await context.Categories.AddRangeAsync(entities);
					await context.SaveChangesAsync();
					await Task.CompletedTask;
				}
			}

			if (!context.Products.Any())
			{
				var jsonData = await File.ReadAllTextAsync("SeedData/Catalog/ProductSeed.json");
				var entities = JsonConvert.DeserializeObject<List<Product>>(jsonData, new JsonSerializerSettings());
				if (entities != null)
				{
					await context.Products.AddRangeAsync(entities);
					await context.SaveChangesAsync();
					await Task.CompletedTask;
				}
			}

			if (!context.Combos.Any())
			{
				var jsonData = await File.ReadAllTextAsync("SeedData/Catalog/ComboSeed.json");
				var entities = JsonConvert.DeserializeObject<List<Combo>>(jsonData, new JsonSerializerSettings());
				if (entities != null)
				{
					await context.Combos.AddRangeAsync(entities);
					await context.SaveChangesAsync();
					await Task.CompletedTask;
				}
			}
		}
	}
}