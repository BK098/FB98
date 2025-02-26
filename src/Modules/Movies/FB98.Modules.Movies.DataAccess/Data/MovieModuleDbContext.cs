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
		}
	}
}