using FB98.Shared.Abstractions.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FB98.Modules.Tickets.Domain.Entities
{
	public class BookingSeat : BaseEntity
	{
		[ForeignKey("Booking")]
		public Guid BookingId { get; set; }
		public Booking? Booking { get; set; }

		[ForeignKey("BookingSeatStatus")]
		public Guid SeatStatusId { get; set; }
		public BookingSeatStatus? SeatStatus { get; set; }

		public Guid SeatId { get; set; }
		public SeatPriceApplication SeatPriceApplication { get; set; } = null!;

		[StringLength(50)]
		public string SeatTypeName { get; set; } = null!;
		[StringLength(10)]
		public string SeatPosition { get; set; } = null!;
		public bool IsReserved { get; set; }
		public decimal Price { get; set; }
	}
}