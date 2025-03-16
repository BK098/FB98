using FB98.Modules.Tickets.Domain.Entities;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FB98.Bootstrapper")]
namespace FB98.Modules.Tickets.DataAccess.Data
{
	internal static class TicketSeeder
	{
		public static async Task SeedDataAsync(TicketModuleDbContext context)
		{
			if (!context.BookingSeatStatuses.Any())
			{
				var jsonData = await File.ReadAllTextAsync("SeedData/Tickets/BookingSeatStatusSeed.json");
				var entities = JsonConvert.DeserializeObject<List<BookingSeatStatus>>(jsonData, new JsonSerializerSettings());
				if (entities != null)
				{
					await context.BookingSeatStatuses.AddRangeAsync(entities);
					await context.SaveChangesAsync();
					await Task.CompletedTask;
				}
			}
			if (!context.BookingStatuses.Any())
			{
				var jsonData = await File.ReadAllTextAsync("SeedData/Tickets/BookingStatusSeed.json");
				var entities = JsonConvert.DeserializeObject<List<BookingStatus>>(jsonData, new JsonSerializerSettings());
				if (entities != null)
				{
					await context.BookingStatuses.AddRangeAsync(entities);
					await context.SaveChangesAsync();
					await Task.CompletedTask;
				}
			}
		}
	}
}