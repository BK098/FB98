using FB98.Modules.Catalog.Domain.Entities;

namespace FB98.Modules.Catalog.Application.Abstractions
{
	public interface ICategoryRepository : IRepository<Category>
	{
		Task<bool> IsCategoryExistsAsync(string categoryName, CancellationToken cancellationToken);
	}
}
