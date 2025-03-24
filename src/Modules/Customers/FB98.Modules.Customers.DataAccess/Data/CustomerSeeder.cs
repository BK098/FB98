using FB98.Modules.Customers.Domain.Entities;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FB98.Bootstrapper")]
namespace FB98.Modules.Customers.DataAccess.Data
{
	internal static class CustomerSeeder
	{
		public static async Task SeedDataAsync(CustomerModuleDbContext context)
		{
			if (!context.Memberships.Any())
			{
				var jsonData = File.ReadAllText("SeedData/Customer/MembershipSeed.json");
				var entities = JsonConvert.DeserializeObject<List<Membership>>(jsonData, new JsonSerializerSettings());
				if (entities != null)
				{
					await context.Memberships.AddRangeAsync(entities);
					await context.SaveChangesAsync();
					await Task.CompletedTask;
				}
			}
		}
	}
}