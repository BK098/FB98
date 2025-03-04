using FB98.Shared.Abstractions.Entities;

namespace FB98.Modules.Shows.Domain.Entities
{
	public class FeatureType : BaseEntity
	{
		public string Name { get; set; }

		public ICollection<Feature> Features { get; set; } = new List<Feature>();
	}
}