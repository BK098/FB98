using FB98.Modules.Customers.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Customers.DataAccess.Data
{
	public sealed class CustomerModuleDbContext : DbContext
	{
		public CustomerModuleDbContext(DbContextOptions<CustomerModuleDbContext> options) : base(options)
		{
		}

		public DbSet<Customer> Customers { get; set; }
		public DbSet<Membership> Memberships { get; set; }
		public DbSet<PointTransaction> PointTransactions { get; set; }


		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.HasDefaultSchema("CustomerModule");
			modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
		}
	}
}