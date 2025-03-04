using FB98.Modules.Movies.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Movies.DataAccess.Data
{
	public class MovieModuleDbContext : DbContext
	{
		public MovieModuleDbContext(DbContextOptions<MovieModuleDbContext> options) : base(options)
		{
		}

		public DbSet<Movie> Movies { get; set; }
		public DbSet<Director> Directors { get; set; }
		public DbSet<Genre> Genres { get; set; }
		public DbSet<Cast> Casts { get; set; }
		public DbSet<MovieCast> MovieCasts { get; set; }
		public DbSet<MovieGenre> MovieGenres { get; set; }
		public DbSet<MovieDirector> MovieDirectors { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.HasDefaultSchema("MovieModule");
			modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

			modelBuilder.Entity<MovieCast>()
				.HasOne(mc => mc.Cast)
				.WithMany(c => c.MovieCasts)
				.HasForeignKey(mc => mc.CastId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<MovieDirector>()
				.HasOne(md => md.Director)
				.WithMany()
				.HasForeignKey(md => md.DirectorId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<MovieGenre>()
				.HasOne(mg => mg.Genre)
				.WithMany()
				.HasForeignKey(mg => mg.GenreId)
				.OnDelete(DeleteBehavior.Restrict);

		}
	}
}