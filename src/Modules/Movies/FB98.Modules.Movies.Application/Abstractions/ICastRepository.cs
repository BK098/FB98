using FB98.Modules.Movies.Domain.Entities;

namespace FB98.Modules.Movies.Application.Abstractions
{
	public interface ICastRepository : IRepository<Cast>
	{
		Task<bool> IsCastExistsAsync(string castName);
	}
}
