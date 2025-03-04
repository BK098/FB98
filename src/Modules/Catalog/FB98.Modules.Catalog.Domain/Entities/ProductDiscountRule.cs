using FB98.Shared.Abstractions.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace FB98.Modules.Catalog.Domain.Entities
{
	public class ProductDiscountRule : BaseEntity
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public decimal Value { get; set; }
		public bool IsCombo { get; set; }
		public bool IsDiscountPercentage { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }

		[ForeignKey("Product")]
		public Guid? ProductId { get; set; }
		public Product? Product { get; set; }

		[ForeignKey("Combo")]
		public Guid? ComboId { get; set; }
		public Combo? Combo { get; set; }

		public bool IsValid()
		{
			var now = DateTime.UtcNow;
			return StartDate <= now && EndDate >= now;
		}
	}
}