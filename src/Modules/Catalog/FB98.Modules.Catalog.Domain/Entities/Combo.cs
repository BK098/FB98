using FB98.Shared.Abstractions.Entities;
using System.Collections.Generic;


namespace FB98.Modules.Catalog.Domain.Entities
{
	public class Combo : BaseEntity
	{
		public string Name { get; set; } = default!;
		public string? Description { get; set; }
		public decimal Price { get; set; }
		public bool IsEnabled { get; set; }

		public ICollection<ComboProduct> ComboProducts { get; set; } = new List<ComboProduct>();
	}
}
