using FB98.Shared.Abstractions.Entities;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FB98.Modules.Catalog.Domain.Entities
{
	public class ComboProduct : BaseEntity
	{
		public int Quantity { get; set; }

		[ForeignKey("Product")]
		public Guid ProductId { get; set; }
		public Product Product { get; set; } = default!;

		[ForeignKey("Combo")]
		public Guid ComboId { get; set; }
		public Combo Combo { get; set; } = default!;
	}
}
