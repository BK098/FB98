using System.ComponentModel.DataAnnotations.Schema;

namespace FB98.Modules.Catalog.Domain.Entities
{
	public class Product : BaseProduct
	{
		[ForeignKey("Category")]
		public Guid CategoryId { get; set; }
		public Category? Category { get; set; }

		public string Unit { get; set; }
		public ICollection<ProductPriceHistory>? PriceHistories { get; set; } = new List<ProductPriceHistory>();
	}
}