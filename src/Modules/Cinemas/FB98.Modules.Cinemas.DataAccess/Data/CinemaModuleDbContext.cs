using FB98.Modules.Cinemas.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Cinemas.DataAccess.Data
{
	public class CinemaModuleDbContext : DbContext
	{
		public CinemaModuleDbContext(DbContextOptions<CinemaModuleDbContext> options) : base(options)
		{
		}

		public DbSet<Cinema> Cinemas { get; set; }
		public DbSet<CinemaHall> CinemaHalls { get; set; }
		public DbSet<CinemaHallSeat> CinemaHallSeats { get; set; }
		public DbSet<SeatType> SeatTypes { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.HasDefaultSchema("CinemaModule");
			modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
		}
	}
}