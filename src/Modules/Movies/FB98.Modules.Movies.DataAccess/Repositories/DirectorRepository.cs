using FB98.Modules.Movies.Application.Abstractions;
using FB98.Modules.Movies.DataAccess.Data;
using FB98.Modules.Movies.Domain.Entities;
using FB98.Shared.Infrastructure.Repositpries;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Movies.DataAccess.Repositories
{
	public class DirectorRepository : BaseRepository<Director, MovieModuleDbContext>, IDirectorRepository
	{
		public DirectorRepository(MovieModuleDbContext context) : base(context)
		{
		}
		public async Task<bool> IsDirectorExistsAsync(string directorName)
		{
			return await GetAll().AnyAsync(c => c.Name == directorName);
		}
	}
}