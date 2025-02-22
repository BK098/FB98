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
		public ProductDiscountRule DiscountRule { get; set; } = default!;

		public static ProductDiscountApplication? ApplyDiscount(BaseProduct product, Guid orderId)
		{
			var discount = product.DiscountRules
				.Where(d => d.IsValid())
				.OrderByDescending(d => d.StartDate)
				.FirstOrDefault();

			if (discount == null)
			{
				return null;
			}

			var discountAmount = discount.IsDiscountPercentage
				? product.Price * discount.Value / 100
				: discount.Value;

			return new ProductDiscountApplication
			{
				ProductId = product.Id,
				OrderId = orderId,
				RuleId = discount.Id,
				AppliedAmount = discountAmount
			};
		}
	}
}