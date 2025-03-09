using FB98.Modules.Tickets.Domain.Entities;

namespace FB98.Modules.Tickets.Application.Abstractions
{
	public interface IBookingSeatLockRepository : IRepository<BookingSeatLock>
	{
		Task<IEnumerable<Guid>> GetLockedSeats(Guid showId, IEnumerable<Guid> seatIds);
		Task<bool> LockSeats(Guid? customerId, Guid showId, ICollection<Guid> seatIds);
		Task<bool> UnlockSeats(Guid? customerId, Guid showId, ICollection<Guid> seatIds);
		Task CleanupExpiredLocks();
	}
}