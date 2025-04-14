using FB98.Modules.Catalog.Domain.Entities;

namespace FB98.Modules.Catalog.Domain.Services
{
	public class ProductDiscountDomainService
	{
		public decimal GetDiscountedPrice(BaseProduct product)
		{
			var activeDiscount = product.DiscountRules?
				.Where(d => d.IsValid())
				.OrderByDescending(d => d.StartDate)
				.FirstOrDefault();

			if (activeDiscount == null) return product.Price;

			return activeDiscount.IsDiscountPercentage
				? product.Price - (product.Price * activeDiscount.Value / 100)
				: product.Price - activeDiscount.Value;
		}

		public ProductDiscountApplication? ApplyDiscount(BaseProduct product, Guid orderDetailId)
		{
			var discount = product.DiscountRules?
				.Where(d => d.IsValid())
				.OrderByDescending(d => d.StartDate)
				.FirstOrDefault();

			if (discount == null) return null;

			var discountAmount = discount.IsDiscountPercentage
				? product.Price * discount.Value / 100
				: discount.Value;

			return new ProductDiscountApplication
			{
				IsCombo = product.IsCombo,
				ProductId = product.Id,
				OrderId = orderDetailId,
				RuleId = discount.Id,
				AppliedAmount = discountAmount
			};
		}
	}
}