using FB98.Modules.Shows.Domain.Entities;

namespace FB98.Modules.Shows.Application.Abstractions
{
	public interface IFeatureRepository : IRepository<Feature>
	{
		Task<bool> IsFeatureExistsAsync(string name, CancellationToken cancellationToken);
	}
}