using FB98.Shared.Abstractions.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FB98.Modules.Tickets.Domain.Entities
{
	public class Booking : BaseEntity
	{
		public Guid UserId { get; set; }
		public string UserName { get; set; } = null!;

		[StringLength(10)]
		public string UserPhone { get; set; } = null!;
		public decimal Amount { get; set; }
		public Guid ShowId { get; set; }
		public Guid HallId { get; set; }

		[StringLength(255)]
		public string HallName { get; set; } = null!;
		public string ShowStart { get; set; } = null!;
		public string ShowEnd { get; set; } = null!;
		public string MovieTitle { get; set; } = null!;

		[ForeignKey("BookingStatus")]
		public Guid StatusId { get; set; }
		public BookingStatus? Status { get; set; }

		public ICollection<BookingSeat> BookingSeats { get; set; } = new List<BookingSeat>();
	}
}