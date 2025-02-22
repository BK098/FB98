using FB98.Modules.Catalog.Domain.Entities;

namespace FB98.Modules.Catalog.Application.Abstractions
{
	public interface IProductRepository : IRepository<Product>
	{
		Task<bool> IsProductExistsAsync(string productName, CancellationToken cancellationToken);
	}
}