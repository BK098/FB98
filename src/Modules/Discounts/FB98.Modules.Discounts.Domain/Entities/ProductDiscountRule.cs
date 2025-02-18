using FB98.Shared.Abstractions.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace FB98.Modules.Discounts.Domain.Entities
{
	public class ProductDiscountRule : BaseEntity
	{
		public string Name { get; set; } = default!;
		public string? Description { get; set; }
		public bool IsDiscountPercentage { get; set; }
		public decimal Value { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public bool IsActive { get; set; }

		public Guid ProductId { get; set; }
	}
}
