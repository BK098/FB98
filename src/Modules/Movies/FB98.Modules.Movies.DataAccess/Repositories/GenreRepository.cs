using FB98.Modules.Movies.Application.Abstractions;
using FB98.Modules.Movies.DataAccess.Data;
using FB98.Modules.Movies.Domain.Entities;
using FB98.Shared.Infrastructure.Repositpries;

namespace FB98.Modules.Movies.DataAccess.Repositories
{
	public class GenreRepository : BaseRepository<Genre, MovieModuleDbContext>, IGenreRepository
	{
		public GenreRepository(MovieModuleDbContext context) : base(context)
		{
		}
	}
}