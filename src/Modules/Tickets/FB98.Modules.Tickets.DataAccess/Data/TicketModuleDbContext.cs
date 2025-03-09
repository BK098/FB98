using FB98.Modules.Tickets.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Tickets.DataAccess.Data
{
	public class TicketModuleDbContext : DbContext
	{
		public TicketModuleDbContext(DbContextOptions<TicketModuleDbContext> options) : base(options)
		{
		}

		public DbSet<Booking> Bookings { get; set; }
		public DbSet<BookingSeat> BookingSeats { get; set; }
		public DbSet<BookingStatus> BookingStatuses { get; set; }
		public DbSet<BookingSeatStatus> BookingSeatStatuses { get; set; }
		public DbSet<SeatPriceRule> SeatPriceRules { get; set; }
		public DbSet<SeatPriceApplication> SeatPriceApplications { get; set; }
		public DbSet<BookingSeatLock> BookingSeatLocks { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.HasDefaultSchema("TicketModule");
			modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<BookingSeatLock>()
				.HasIndex(e => e.ShowId).IsUnique();
			modelBuilder.Entity<BookingSeatLock>()
				.HasIndex(e => e.SeatId).IsUnique();

		}
	}
}