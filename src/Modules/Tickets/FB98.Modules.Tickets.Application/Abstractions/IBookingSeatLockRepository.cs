using FB98.Modules.Tickets.Domain.Entities;

namespace FB98.Modules.Tickets.Application.Abstractions
{
	public interface IBookingSeatLockRepository : IRepository<BookingSeatLock>
	{
		Task<IEnumerable<Guid>> GetLockedSeats(Guid showId, IEnumerable<Guid> seatIds);
		Task<bool> LockSeats(Guid customerId, Guid showId, ICollection<Guid> seatIds, DateTime expirationTime);
		Task<bool> UnlockSeats(Guid customerId, Guid showId, ICollection<Guid> seatIds);
		Task<List<BookingSeatLock>?> CleanupExpiredLocks();
		Task<List<BookingSeatLock>> GetLockedSeatsByUser(Guid userId, Guid showId);
		Task<List<BookingSeatLock>> GetLockedSeatsByShow(Guid showId);
		Task<bool> ExtendLockForPayment(Guid userId, Guid showId, List<Guid> seatIds);
		Task<bool> ReleaseSeatsAfterSuccessfulPayment(Guid bookingId);
	}
}