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
			//var movie = await _context.Movies
			//	.Where(m => m.Id == id)
			//	.Select(m => new Movie
			//	{
			//		Id = m.Id,
			//		Title = m.Title,
			//		AgeRating = m.AgeRating,
			//		ReleaseDate = m.ReleaseDate,
			//		RuntimeMinutes = m.RuntimeMinutes,
			//		IsPublished = m.IsPublished,
			//		TrailerLink = m.TrailerLink,
			//		HeaderImage = m.HeaderImage,
			//		PosterImage = m.PosterImage,
			//		Description = m.Description,
			//		Language = m.Language,
			//		IsVietSub = m.IsVietSub,
			//		Genres = m.Genres.Select(g => new MovieGenre
			//		{
			//			GenreId = g.GenreId,
			//			Genre = new Genre
			//			{ Name = g.Genre.Name }
			//		}).ToList(),
			//		Directors = m.Directors.Select(d => new MovieDirector
			//		{
			//			DirectorId = d.DirectorId,
			//			Director = new Director
			//			{ Name = d.Director.Name }
			//		}).ToList(),
			//		Casts = m.Casts.Select(c => new MovieCast
			//		{
			//			CastId = c.CastId,
			//			Cast = new Cast
			//			{ Name = c.Cast.Name }
			//		}).ToList()
			//	})
			//	.FirstOrDefaultAsync();

			//return movie;
			var movie = await _context.Movies
				.Include(x => x.Directors).ThenInclude(x => x.Director)
				.Include(x => x.Genres).ThenInclude(x => x.Genre)
				.Include(x => x.Casts).ThenInclude(x => x.Cast)
				.FirstOrDefaultAsync(x => x.Id == id);
			return movie;
		}
	}
}