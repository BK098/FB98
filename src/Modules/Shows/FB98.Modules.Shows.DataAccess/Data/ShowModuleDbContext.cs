using FB98.Modules.Shows.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Shows.DataAccess.Data
{
	public class ShowModuleDbContext : DbContext
	{
		public ShowModuleDbContext(DbContextOptions<ShowModuleDbContext> options) : base(options)
		{
		}

		public DbSet<Show> Shows { get; set; }
		public DbSet<ShowFeature> ShowFeatures { get; set; }
		public DbSet<ShowStatus> ShowStatuses { get; set; }
		public DbSet<Feature> Features { get; set; }
		public DbSet<FeatureType> FeatureTypes { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.HasDefaultSchema("ShowModule");
			modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

			modelBuilder.Entity<ShowFeature>()
				.HasOne(sf => sf.Feature)
				.WithMany()
				.HasForeignKey(sf => sf.FeatureId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Feature>()
				.HasOne(f => f.FeatureType)
				.WithMany(ft => ft.Features)
				.HasForeignKey(f => f.FeatureTypeId)
				.OnDelete(DeleteBehavior.Restrict);

		}
	}
}