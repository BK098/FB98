using FB98.Modules.Cinemas.Domain.Entities;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FB98.Bootstrapper")]
namespace FB98.Modules.Cinemas.DataAccess.Data
{
	internal class CinemaSeeder
	{
		public static async Task SeedDataAsync(CinemaModuleDbContext context)
		{
			if (!context.SeatTypes.Any())
			{
				var jsonData = await File.ReadAllTextAsync("SeedData/Cinema/SeatTypeSeed.json");
				var entities = JsonConvert.DeserializeObject<List<SeatType>>(jsonData, new JsonSerializerSettings());
				if (entities != null)
				{
					await context.SeatTypes.AddRangeAsync(entities);
					await context.SaveChangesAsync();
					await Task.CompletedTask;
				}
			}

			if (!context.Cinemas.Any())
			{
				var jsonData = await File.ReadAllTextAsync("SeedData/Cinema/CinemaSeed.json");
				var entities = JsonConvert.DeserializeObject<List<Cinema>>(jsonData, new JsonSerializerSettings());
				if (entities != null)
				{
					await context.Cinemas.AddRangeAsync(entities);
					await context.SaveChangesAsync();
					await Task.CompletedTask;
				}
			}

			if (!context.CinemaHalls.Any())
			{
				var jsonData = await File.ReadAllTextAsync("SeedData/Cinema/CinemaHallSeed.json");
				var entities = JsonConvert.DeserializeObject<List<CinemaHall>>(jsonData, new JsonSerializerSettings());
				if (entities != null)
				{
					await context.CinemaHalls.AddRangeAsync(entities);
					await context.SaveChangesAsync();
					await Task.CompletedTask;
				}
			}

			if (!context.CinemaHallSeats.Any())
			{
				var jsonData = await File.ReadAllTextAsync("SeedData/Cinema/CinemaHallSeatSeed.json");
				var entities = JsonConvert.DeserializeObject<List<CinemaHallSeat>>(jsonData, new JsonSerializerSettings());
				if (entities != null)
				{
					foreach (var seat in entities)
					{
						seat.SetSeatPosition(seat.SeatRow, seat.SeatColumn);
					}
					var totalRecords = entities.Count();
					const int bacthSize = 50;
					var batchCount = totalRecords / bacthSize + (totalRecords % bacthSize > 0 ? 1 : 0);
					for (var i = 0; i < batchCount; i++)
					{
						var batch = entities.Skip(i * bacthSize).Take(bacthSize).ToList();
						await context.CinemaHallSeats.AddRangeAsync(batch);
						await context.SaveChangesAsync();
						await Task.Delay(50);
					}

					await Task.CompletedTask;
				}
			}
		}
	}
}