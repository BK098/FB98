using FB98.Shared.Abstractions.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FB98.Modules.Catalog.Domain.Entities
{
	public class Product : BaseEntity
	{
		public string Name { get; set; } = default!;
		public string? Description { get; set; }
		public decimal Price { get; set; }
		public string? Image { get; set; }
		public bool IsEnabled { get; set; }

		[ForeignKey("Category")]
		public Guid CategoryId { get; set; }
		public Category Category { get; set; } = default!;

		public ICollection<ComboProduct> ComboProducts { get; set; } = new List<ComboProduct>();
	}
}
