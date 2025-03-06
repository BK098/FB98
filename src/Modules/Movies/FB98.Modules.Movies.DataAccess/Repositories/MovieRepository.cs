using FB98.Modules.Movies.Application.Abstractions;
using FB98.Modules.Movies.DataAccess.Data;
using FB98.Modules.Movies.Domain.Entities;
using FB98.Shared.Infrastructure.Repositpries;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Movies.DataAccess.Repositories
{
	public class MovieRepository : BaseRepository<Movie, MovieModuleDbContext>, IMovieRepository
	{
		public MovieRepository(MovieModuleDbContext context) : base(context)
		{
		}

		public override async Task<Movie?> GetByIdAsync(Guid? id)
		{
			var movie = await _context.Movies
				.Include(x => x.Directors).ThenInclude(x => x.Director)
				.Include(x => x.Genres).ThenInclude(x => x.Genre)
				.Include(x => x.Casts).ThenInclude(x => x.Cast)
				.FirstOrDefaultAsync(x => x.Id == id);
			return movie;
		}
	}
}