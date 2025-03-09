using FB98.Shared.Abstractions.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace FB98.Modules.Tickets.Domain.Entities
{
	public class Booking : BaseEntity
	{
		public Guid? CustomerId { get; set; }
		public decimal DiscountPercentage { get; private set; } // phần trăm giảm giá cho cả đơn hàng
		public decimal Amount { get; set; }
		public decimal SubAmount { get; set; }

		[ForeignKey("BookingStatus")]
		public Guid StatusId { get; set; }
		public BookingStatus Status { get; set; }

		public ICollection<BookingSeat> BookingSeats { get; set; } = new List<BookingSeat>();

		public void SetDiscountPercentage()
		{
			if (SubAmount == 0)
			{
				DiscountPercentage = 0;
				return;
			}

			var discountPrercentage = (SubAmount - Amount) / SubAmount * 100;
			DiscountPercentage = Math.Round(discountPrercentage, 2);
		}
	}
}