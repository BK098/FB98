using FB98.Shared.Abstractions.Entities;
using System.ComponentModel.DataAnnotations;

namespace FB98.Modules.Shows.Domain.Entities
{
	public class FeatureType : BaseEntity
	{
		[StringLength(255)]
		public string Name { get; set; } = null!;

		public ICollection<Feature> Features { get; set; } = new List<Feature>();
	}
}