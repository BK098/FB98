using FB98.Shared.Abstractions.Entities;
using System.ComponentModel.DataAnnotations;

namespace FB98.Modules.Customers.Domain.Entities
{
	public class Membership : BaseEntity
	{
		[StringLength(255)]
		public string LevelName { get; set; } = null!;
		public decimal TotalAmountForUpgrade { get; set; }
		public int DiscountRate { get; set; } = 0;
	}
}