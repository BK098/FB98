using FB98.Shared.Abstractions.Entities;

namespace FB98.Modules.Tickets.Domain.Entities
{
	public class BookingSeatLock : BaseEntity
	{
		public Guid ShowId { get; set; }
		public Guid SeatId { get; set; }
		public Guid CustomerId { get; set; }
		public DateTime LockedUntil { get; set; }
		public bool IsPaymentInProgress { get; set; }
	}
}