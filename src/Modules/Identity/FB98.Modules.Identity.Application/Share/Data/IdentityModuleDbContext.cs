using FB98.Modules.Identity.Application.Share.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Identity.Application.Share.Data
{
	public class IdentityModuleDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
	{
		public IdentityModuleDbContext(DbContextOptions<IdentityModuleDbContext> options) : base(options) { }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<IdentityUserLogin<Guid>>(entity =>
			{
				entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });
			});

			modelBuilder.Entity<IdentityUserRole<Guid>>(entity =>
			{
				entity.HasKey(e => new { e.UserId, e.RoleId });
			});

			modelBuilder.Entity<IdentityUserToken<Guid>>(entity =>
			{
				entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });
			});
			modelBuilder.HasDefaultSchema("IdentityModule");
			modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
		}
	}
}