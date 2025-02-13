using FB98.Shared.Abstractions.Entities;
using System.Collections.Generic;

namespace FB98.Modules.Catalog.Domain.Entities
{
	public class Category : BaseEntity
	{
		public string Name { get; set; } = default!;

		public ICollection<Product> Products { get; set; } = new List<Product>();
	}
}
