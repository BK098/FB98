using FB98.Shared.Abstractions.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace FB98.Modules.Shows.Domain.Entities
{
	public class Feature : BaseEntity
	{
		public string Name { get; set; }
		public string Description { get; set; }

		[ForeignKey("FeatureType")]
		public Guid FeatureTypeId { get; set; }
		public FeatureType FeatureType { get; set; }
	}
}