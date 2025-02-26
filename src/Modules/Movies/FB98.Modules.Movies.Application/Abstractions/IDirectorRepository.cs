using FB98.Modules.Movies.Domain.Entities;

namespace FB98.Modules.Movies.Application.Abstractions
{
	public interface IDirectorRepository : IRepository<Director>
	{
		Task<bool> IsDirectorExistsAsync(string directorName);
	}
}