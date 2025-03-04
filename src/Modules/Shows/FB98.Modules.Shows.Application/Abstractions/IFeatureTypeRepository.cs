using FB98.Modules.Shows.Domain.Entities;

namespace FB98.Modules.Shows.Application.Abstractions
{
	public interface IFeatureTypeRepository : IRepository<FeatureType>
	{
		Task<bool> IsFeatureTypeExistsAsync(string name, CancellationToken cancellationToken);
	}
}