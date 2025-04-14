using FB98.Shared.Abstractions.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace FB98.Modules.Catalog.Domain.Entities
{
	public class ProductDiscountApplication : BaseEntity
	{
		public bool IsCombo { get; set; }
		public Guid ProductId { get; set; }
		public Guid OrderId { get; set; }
		public decimal AppliedAmount { get; set; }

		[ForeignKey("DiscountRule")]
		public Guid RuleId { get; set; }
		public ProductDiscountRule? DiscountRule { get; set; }
	}
}