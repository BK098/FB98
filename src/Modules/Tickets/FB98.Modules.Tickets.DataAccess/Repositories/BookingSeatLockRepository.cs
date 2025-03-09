using FB98.Modules.Tickets.Application.Abstractions;
using FB98.Modules.Tickets.DataAccess.Data;
using FB98.Modules.Tickets.Domain.Entities;
using FB98.Shared.Infrastructure.Repositpries;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Tickets.DataAccess.Repositories
{
	public class BookingSeatLockRepository : BaseRepository<BookingSeatLock, TicketModuleDbContext>, IBookingSeatLockRepository
	{
		public BookingSeatLockRepository(TicketModuleDbContext context) : base(context)
		{
		}

		public async Task<IEnumerable<Guid>> GetLockedSeats(Guid showId, IEnumerable<Guid> seatIds)
		{
			return await _context.Set<BookingSeatLock>()
				.Where(s => s.ShowId == showId && seatIds.Contains(s.SeatId) && s.LockedUntil > DateTime.UtcNow)
				.Select(s => s.SeatId)
				.ToListAsync();
		}

		public async Task<bool> LockSeats(Guid? customerId, Guid showId, ICollection<Guid> seatIds)
		{
			var lockedSeats = await GetLockedSeats(showId, seatIds);
			if (lockedSeats.Any())
			{
				return false;
			}

			var locks = seatIds.Select(seatId => new BookingSeatLock
			{
				ShowId = showId,
				SeatId = seatId,
				CustomerId = customerId,
				LockedUntil = DateTime.UtcNow.AddMinutes(15)
			});
			await using var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				await _context.BookingSeatLocks.AddRangeAsync(locks);
				var result = await _context.SaveChangesAsync();
				await transaction.CommitAsync();

				return result > 0;
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				//_logger.LogError(ex, "Error occurred while locking seats");
				return false;
			}
		}

		public async Task<bool> UnlockSeats(Guid? customerId, Guid showId, ICollection<Guid> seatIds)
		{
			var locks = await _context.Set<BookingSeatLock>()
				.Where(s => s.CustomerId == customerId && s.ShowId == showId && seatIds.Contains(s.SeatId))
				.ToListAsync();

			if (!locks.Any())
			{
				return false;
			}

			_context.BookingSeatLocks.RemoveRange(locks);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task CleanupExpiredLocks()
		{
			var expiredLocks = await _context.Set<BookingSeatLock>()
				.Where(s => s.LockedUntil < DateTime.UtcNow)
				.ToListAsync();

			if (expiredLocks.Any())
			{
				_context.Set<BookingSeatLock>().RemoveRange(expiredLocks);
				await _context.SaveChangesAsync();
			}
		}
	}
}