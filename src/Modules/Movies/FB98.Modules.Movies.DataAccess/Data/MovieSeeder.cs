using FB98.Modules.Movies.Domain.Entities;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FB98.Bootstrapper")]
namespace FB98.Modules.Movies.DataAccess.Data
{
	internal class MovieSeeder
	{
		public static async Task SeedDataAsync(MovieModuleDbContext context)
		{
			if (!context.Directors.Any())
			{
				var jsonData = await File.ReadAllTextAsync("SeedData/Movies/DirectorSeed.json");
				var entities = JsonConvert.DeserializeObject<List<Director>>(jsonData, new JsonSerializerSettings());
				if (entities != null)
				{
					await context.Directors.AddRangeAsync(entities);
					await context.SaveChangesAsync();
					await Task.CompletedTask;
				}
			}
			if (!context.Genres.Any())
			{
				var jsonData = await File.ReadAllTextAsync("SeedData/Movies/GenreSeed.json");
				var entities = JsonConvert.DeserializeObject<List<Genre>>(jsonData, new JsonSerializerSettings());
				if (entities != null)
				{
					await context.Genres.AddRangeAsync(entities);
					await context.SaveChangesAsync();
					await Task.CompletedTask;
				}
			}
			if (!context.Casts.Any())
			{
				var jsonData = await File.ReadAllTextAsync("SeedData/Movies/CastSeed.json");
				var entities = JsonConvert.DeserializeObject<List<Cast>>(jsonData, new JsonSerializerSettings());
				if (entities != null)
				{
					await context.Casts.AddRangeAsync(entities);
					await context.SaveChangesAsync();
					await Task.CompletedTask;
				}
			}
			if (!context.Movies.Any())
			{
				var jsonData = await File.ReadAllTextAsync("SeedData/Movies/MovieSeed.json");
				var entities = JsonConvert.DeserializeObject<List<Movie>>(jsonData, new JsonSerializerSettings());
				if (entities != null)
				{
					await context.Movies.AddRangeAsync(entities);
					await context.SaveChangesAsync();
					await Task.CompletedTask;
				}
			}
		}
	}
}