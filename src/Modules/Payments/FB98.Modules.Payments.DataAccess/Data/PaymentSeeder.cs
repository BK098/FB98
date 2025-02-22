using FB98.Modules.Payments.Domain.Entities;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FB98.Bootstrapper")]
namespace FB98.Modules.Payments.DataAccess.Data
{
	internal class PaymentSeeder
	{
		public static async Task SeedDataAsync(PaymentModuleDbContext context)
		{
			if (!context.PaymentMethods.Any())
			{
				var jsonData = await File.ReadAllTextAsync("SeedData/Payments/PaymentMethodSeed.json");
				var entities = JsonConvert.DeserializeObject<List<PaymentMethod>>(jsonData, new JsonSerializerSettings());
				if (entities != null)
				{
					await context.PaymentMethods.AddRangeAsync(entities);
					await context.SaveChangesAsync();
					await Task.CompletedTask;
				}
			}
			if (!context.PaymentStatuses.Any())
			{
				var jsonData = await File.ReadAllTextAsync("SeedData/Payments/PaymentStatusSeed.json");
				var entities = JsonConvert.DeserializeObject<List<PaymentStatus>>(jsonData, new JsonSerializerSettings());
				if (entities != null)
				{
					await context.PaymentStatuses.AddRangeAsync(entities);
					await context.SaveChangesAsync();
					await Task.CompletedTask;
				}
			}
		}
	}
}
