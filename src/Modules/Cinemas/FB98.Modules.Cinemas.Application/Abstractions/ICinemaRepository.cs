using FB98.Modules.Cinemas.Domain.Entities;

namespace FB98.Modules.Cinemas.Application.Abstractions
{
	public interface ICinemaRepository : IRepository<Cinema>
	{
		Task<bool> IsCinemaExisted(string cinemaName);
	}
}