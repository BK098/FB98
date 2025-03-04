using FB98.Modules.Shows.Domain.Entities;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FB98.Bootstrapper")]
namespace FB98.Modules.Shows.DataAccess.Data
{
	internal static class ShowSeeder
	{
		public static async Task SeedDataAsync(ShowModuleDbContext context)
		{
			if (!context.ShowStatuses.Any())
			{
				var jsonData = await File.ReadAllTextAsync("SeedData/Shows/ShowStatusSeed.json");
				var entities = JsonConvert.DeserializeObject<List<ShowStatus>>(jsonData, new JsonSerializerSettings());
				if (entities != null)
				{
					await context.ShowStatuses.AddRangeAsync(entities);
					await context.SaveChangesAsync();
					await Task.CompletedTask;
				}
			}

			if (!context.FeatureTypes.Any())
			{
				var jsonData = await File.ReadAllTextAsync("SeedData/Shows/FeatureTypeSeed.json");
				var entities = JsonConvert.DeserializeObject<List<FeatureType>>(jsonData, new JsonSerializerSettings());
				if (entities != null)
				{
					await context.FeatureTypes.AddRangeAsync(entities);
					await context.SaveChangesAsync();
					await Task.CompletedTask;
				}
			}

			if (!context.Features.Any())
			{
				var jsonData = await File.ReadAllTextAsync("SeedData/Shows/FeatureSeed.json");
				var entities = JsonConvert.DeserializeObject<List<Feature>>(jsonData, new JsonSerializerSettings());
				if (entities != null)
				{
					await context.Features.AddRangeAsync(entities);
					await context.SaveChangesAsync();
					await Task.CompletedTask;
				}
			}
		}
	}
}