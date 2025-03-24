using FB98.Shared.Abstractions.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace FB98.Modules.Tickets.Domain.Entities
{
	public class Booking : BaseEntity
	{
		public Guid UserId { get; set; }
		public string UserName { get; set; }
		public string UserPhone { get; set; }
		public decimal Amount { get; set; }
		public Guid ShowId { get; set; }
		public Guid HallId { get; set; }
		public string HallName { get; set; }
		public string ShowStart { get; set; }
		public string ShowEnd { get; set; }
		public string MovieTitle { get; set; }

		[ForeignKey("BookingStatus")]
		public Guid StatusId { get; set; }
		public BookingStatus Status { get; set; }

		public ICollection<BookingSeat> BookingSeats { get; set; } = new List<BookingSeat>();
	}
}