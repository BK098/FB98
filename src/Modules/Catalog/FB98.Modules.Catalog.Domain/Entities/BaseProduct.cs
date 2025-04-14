using FB98.Shared.Abstractions.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace FB98.Modules.Catalog.Domain.Entities
{
	public class BaseProduct : BaseEntity
	{
		public string Name { get; set; } = null!;
		public string? Description { get; set; }
		public decimal Price { get; set; }
		public string? Image { get; set; }
		public bool IsEnabled { get; set; }

		public ICollection<ComboProduct> ComboProducts { get; set; } = new List<ComboProduct>();
		public ICollection<ProductDiscountRule>? DiscountRules { get; set; } = new List<ProductDiscountRule>();

		[NotMapped]
		public bool IsCombo => this is Combo;

		public decimal GetDiscountedPrice()
		{
			var activeDiscount = DiscountRules?.Where(d => d.IsValid())
				.OrderByDescending(d => d.StartDate)
				.FirstOrDefault();

			if (activeDiscount == null)
			{
				return 0;
			}

			return activeDiscount.IsDiscountPercentage
				? Price * (1 - activeDiscount.Value / 100)
				: Price - activeDiscount.Value;
		}
	}
}