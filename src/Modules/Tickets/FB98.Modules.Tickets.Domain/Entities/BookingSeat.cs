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
		public Guid SeatStatusId { get; set; }
		public BookingSeatStatus SeatStatus { get; set; }

		public SeatPriceApplication SeatPriceApplication { get; set; }

		public Guid SeatId { get; set; }
		public bool IsReserved { get; set; }
		public decimal Price { get; set; }

		//public ICollection<BookingSeatLock> SeatLocks { get; set; } = new List<BookingSeatLock>();
	}
}