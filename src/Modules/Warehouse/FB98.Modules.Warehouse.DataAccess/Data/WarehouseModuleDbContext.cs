using FB98.Modules.Warehouse.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Warehouse.DataAccess.Data
{
	public class WarehouseModuleDbContext : DbContext
	{
		/// <inheritdoc />
		public WarehouseModuleDbContext(DbContextOptions<WarehouseModuleDbContext> options) : base(options)
		{
		}

		public DbSet<Inventory> Inventories { get; set; }
		public DbSet<InventoryTransaction> InventoryTransactions { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.HasDefaultSchema("WarehouseModule");
			modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

			modelBuilder.Entity<InventoryTransaction>()
				.HasOne(it => it.Inventory)
				.WithMany(i => i.Transactions)
				.HasForeignKey(it => it.InventoryId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}