using FB98.Modules.Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FB98.Bootstrapper")]
namespace FB98.Modules.Identity.DataAccess.Data
{
	internal static class IdentitySeeder
	{
		public static async Task SeedDataAsync(IdentityModuleDbContext context)
		{
			if (!context.Roles.Any())
			{
				var roles = new List<IdentityRole<Guid>>
				{
					new()
					{
						Id = Guid.Parse("1bc0fc56-605b-47fc-a090-3863e976cedc"),
						Name = "Administrator",
						NormalizedName = "ADMINISTRATOR"
					},
					new()
					{
						Id = Guid.Parse("e25ada12-df9a-49c9-ae13-26076ccdcb0e"),
						Name = "Customer",
						NormalizedName = "CUSTOMER"
					},
					new()
					{
						Id = Guid.Parse("67150dc6-66cd-48d7-82a3-dc29c863c5b3"),
						Name = "Guest",
						NormalizedName = "GUEST"
					}
				};
				await context.Roles.AddRangeAsync(roles);
				await context.SaveChangesAsync();
			}
			if (!context.Users.Any())
			{
				var user = new AppUser
				{
					Id = Guid.Parse("d6c0fee8-9482-4f0c-884d-03017723990d"),
					Firstname = "Min",
					Lastname = "Ạt",
					Age = 100,
					BirthOfDate = new DateOnly(2000, 1, 1),
					UserName = "admin@admin.com",
					Email = "admin@admin.com",
					NormalizedUserName = "ADMIN@ADMIN.COM",
					NormalizedEmail = "ADMIN@ADMIN.COM",
					PasswordHash = "AQAAAAIAAYagAAAAEMYNqBINXmZwVXPVgfwaXJEn6r7t6gZSzKIoRUlzF3A0fyk30d0YXU5agqsv/lE1ow==",
					SecurityStamp = "G5FVKHHWIUXXYYYBQXGNRWK75HNO4IJE",
					ConcurrencyStamp = "1db9b7f7-4e06-481b-8145-92a0f29c45ed",
				};
				var userRole = new IdentityUserRole<Guid>
				{
					RoleId = Guid.Parse("1bc0fc56-605b-47fc-a090-3863e976cedc"),
					UserId = Guid.Parse("d6c0fee8-9482-4f0c-884d-03017723990d")
				};
				await context.Users.AddAsync(user);
				await context.UserRoles.AddAsync(userRole);
				await context.SaveChangesAsync();
			}
		}
	}
}