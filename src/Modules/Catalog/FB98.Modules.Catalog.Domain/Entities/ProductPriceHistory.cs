using FB98.Shared.Abstractions.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace FB98.Modules.Catalog.Domain.Entities
{
	public class ProductPriceHistory : BaseEntity
	{
		[ForeignKey("Product")]
		public Guid ProductId { get; set; }
		public Product Product { get; set; } = null!;

		public decimal OldPrice { get; set; }
		public decimal NewPrice { get; set; }
		public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
	}
}