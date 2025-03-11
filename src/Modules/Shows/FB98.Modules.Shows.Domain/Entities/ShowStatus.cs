using FB98.Shared.Abstractions.Entities;

namespace FB98.Modules.Shows.Domain.Entities
{
	public class ShowStatus : BaseEntity
	{
		public string Name { get; set; }
		public string Description { get; set; }
	}
}