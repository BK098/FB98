using FB98.Modules.Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Identity.DataAccess.Data
{
	public class IdentityModuleDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
	{
		public IdentityModuleDbContext(DbContextOptions<IdentityModuleDbContext> options) : base(options) { }
		public virtual DbSet<TokenStore> RefreshTokens { get; set; }
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

			modelBuilder.Entity<TokenStore>()
				.HasOne(ts => ts.AppUser)
				.WithMany(u => u.TokenStores)
				.HasForeignKey(ts => ts.UserId)
				.OnDelete(DeleteBehavior.Cascade);
			modelBuilder.HasDefaultSchema("IdentityModule");
			modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
		}
	}
}