using FB98.Shared.Abstractions.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace FB98.Modules.Tickets.Domain.Entities
{
	public class SeatPriceApplication : BaseEntity
	{
		[ForeignKey("SeatPriceRule")]
		public Guid SeatPriceRuleId { get; set; }
		public SeatPriceRule SeatPriceRule { get; set; }

		[ForeignKey("BookingSeat")]
		public Guid BookingSeatId { get; set; }
		public BookingSeat BookingSeat { get; set; }

		public decimal AppliedPrice { get; set; }
	}
}