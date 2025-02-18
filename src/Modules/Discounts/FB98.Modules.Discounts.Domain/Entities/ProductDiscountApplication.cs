using FB98.Shared.Abstractions.Entities;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FB98.Modules.Discounts.Domain.Entities
{
	public class ProductDiscountApplication : BaseEntity
	{
		[ForeignKey("ProductDiscountRule")]
		public Guid RuleId { get; set; }
		public ProductDiscountRule Rule { get; set; } = default!;

		public Guid OrderItemId { get; set; }

		public decimal AppliedAmount { get; set; }
	}
}
