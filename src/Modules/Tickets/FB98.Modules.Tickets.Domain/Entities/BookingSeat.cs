using FB98.Shared.Abstractions.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace FB98.Modules.Tickets.Domain.Entities
{
	public class BookingSeat : BaseEntity
	{
		[ForeignKey("Booking")]
		public Guid BookingId { get; set; }
		public Booking Booking { get; set; }

		[ForeignKey("BookingSeatStatus")]
		public Guid StatusId { get; set; }
		public BookingSeatStatus Status { get; set; }

		[ForeignKey("SeatPriceApplication")]
		public Guid PriceApplicationId { get; set; }
		public SeatPriceApplication SeatPriceApplication { get; set; }

		public Guid ShowId { get; set; }
		public Guid SeatId { get; set; }
		public bool IsReserved { get; set; }
		public decimal Price { get; private set; }

		public void SetPrice(decimal price)
		{
			Price = price;
		}
	}
}