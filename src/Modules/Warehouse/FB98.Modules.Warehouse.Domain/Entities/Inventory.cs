using FB98.Shared.Abstractions.Entities;
using System;

namespace FB98.Modules.Warehouse.Domain.Entities
{
	public class Inventory : BaseEntity
	{
		public Guid ProductId { get; set; }
		public int Quantity { get; set; }
	}
}
