using FB98.Modules.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Catalog.DataAccess.Data
{
	public class CatalogModuleDbContext : DbContext
	{
		public CatalogModuleDbContext(DbContextOptions<CatalogModuleDbContext> options) : base(options)
		{
		}

		public DbSet<Category> Categories { get; set; }
		public DbSet<Product> Products { get; set; }
		public DbSet<Combo> Combos { get; set; }
		public DbSet<ComboProduct> ComboProducts { get; set; }
		public DbSet<ProductDiscountRule> ProductDiscountRules { get; set; }
		public DbSet<ProductDiscountApplication> ProductDiscountApplications { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.HasDefaultSchema("CatalogModule");
			modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
		}
	}
}
