using FB98.Modules.Orders.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Orders.DataAccess.Data
{
	public class OrderModuleDbContext : DbContext
	{
		public OrderModuleDbContext(DbContextOptions<OrderModuleDbContext> options) : base(options)
		{
		}

		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderItem> OrderItems { get; set; }
		public DbSet<OrderStatus> OrderStatuses { get; set; }
		public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.HasDefaultSchema("OrderModule");
			modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

			modelBuilder.Entity<OrderItem>()
				.HasOne(mc => mc.Order)
				.WithMany(c => c.OrderItems)
				.HasForeignKey(mc => mc.OrderId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<OrderStatusHistory>()
				.HasOne(mc => mc.Order)
				.WithMany(c => c.StatusHistories)
				.HasForeignKey(mc => mc.OrderId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}