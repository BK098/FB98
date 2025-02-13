using FB98.Modules.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Catalog.DataAccess.Data
{
	public class CatalogModuleDbContext : DbContext
	{
		public CatalogModuleDbContext(DbContextOptions<CatalogModuleDbContext> options) : base(options)
		{
		}

		public virtual DbSet<Category> Categories { get; set; }
		public virtual DbSet<Product> Products { get; set; }
		public virtual DbSet<Combo> Combos { get; set; }
		public virtual DbSet<ComboProduct> ComboProducts { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.HasDefaultSchema("CatalogModule");
			modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
		}
	}
}
