using FB98.Modules.Payments.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Payments.DataAccess.Data
{
	public class PaymentModuleDbContext : DbContext
	{
		public PaymentModuleDbContext(DbContextOptions<PaymentModuleDbContext> options) : base(options)
		{
		}

		public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
		public DbSet<PaymentStatus> PaymentStatuses { get; set; }
		public DbSet<PaymentMethod> PaymentMethods { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.HasDefaultSchema("PaymentsModule");
			modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
		}
	}
}