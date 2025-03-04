using FB98.Shared.Abstractions.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace FB98.Modules.Shows.Domain.Entities
{
	public class ShowFeature : BaseEntity
	{
		[ForeignKey("Show")]
		public Guid ShowId { get; set; }
		public Show Show { get; set; }

		[ForeignKey("Feature")]
		public Guid FeatureId { get; set; }
		public Feature Feature { get; set; }
	}
}