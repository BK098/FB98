using FB98.Modules.Tickets.Application.Abstractions;
using FB98.Modules.Tickets.DataAccess.Data;
using FB98.Modules.Tickets.Domain.Entities;
using FB98.Shared.Abstractions.StatusConstants;
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
				.Where(s => s.ShowId == showId
							&& seatIds.Contains(s.SeatId)
							&& s.LockedUntil > DateTime.UtcNow)
				.Select(s => s.SeatId)
				.ToListAsync();
		}

		public async Task<bool> LockSeats(Guid customerId, Guid showId, ICollection<Guid> seatIds)
		{
			var executionStrategy = _context.Database.CreateExecutionStrategy();
			return await executionStrategy.ExecuteAsync(async () =>
			{
				await using var transaction = await _context.Database.BeginTransactionAsync();

				try
				{
					var bookedSeats = await _context.BookingSeats
						.Where(bs => seatIds.Contains(bs.SeatId) && bs.SeatStatusId == BookingSeatStatusConstants.Available)
						.ToListAsync();

					if (bookedSeats.Any())
					{
						return false; // Không cho phép khóa nếu ghế đã được thanh toán
					}

					var existingLocks = await _context.BookingSeatLocks
						.Where(s => s.ShowId == showId && seatIds.Contains(s.SeatId) && s.LockedUntil > DateTime.UtcNow)
						.ToListAsync();

					if (existingLocks.Any())
					{
						return false; // Không cho phép khóa nếu đã có khóa hiện tại
					}

					var locks = seatIds.Select(seatId => new BookingSeatLock
					{
						ShowId = showId,
						SeatId = seatId,
						CustomerId = customerId,
						LockedUntil = DateTime.UtcNow.AddMinutes(5),
						IsPaymentInProgress = false
					});

					await _context.BookingSeatLocks.AddRangeAsync(locks);
					var result = await _context.SaveChangesAsync();
					await transaction.CommitAsync();
					return result > 0;
				}
				catch (DbUpdateException dbEx)
				{
					await transaction.RollbackAsync();
					return false;
				}
				catch (Exception ex)
				{
					await transaction.RollbackAsync();
					return false;
				}
			});
		}


		public async Task<bool> UnlockSeats(Guid customerId, Guid showId, ICollection<Guid> seatIds)
		{
			var lockSeats = await _context.Set<BookingSeatLock>()
				.Where(s => s.CustomerId == customerId &&
							s.ShowId == showId &&
							seatIds.Contains(s.SeatId))
				.ToListAsync();

			if (!lockSeats.Any())
			{
				return false;
			}

			_context.BookingSeatLocks.RemoveRange(lockSeats);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task CleanupExpiredLocks()
		{
			var now = DateTime.UtcNow;

			var expiredLocks = await _context.BookingSeatLocks
				.Where(s => s.LockedUntil < now)
				.ToListAsync();

			if (expiredLocks.Any())
			{
				_context.Set<BookingSeatLock>().RemoveRange(expiredLocks);
				await _context.SaveChangesAsync();
			}

			var paymentTimeoutLocks = await _context.BookingSeatLocks
				.Where(s => s.LockedUntil < now.AddMinutes(-15) && s.IsPaymentInProgress)
				.ToListAsync();

			if (paymentTimeoutLocks.Any())
			{
				_context.BookingSeatLocks.RemoveRange(paymentTimeoutLocks);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<List<BookingSeatLock>> GetLockedSeatsByUser(Guid userId, Guid showId)
		{
			return await _context.BookingSeatLocks
				.Where(s => s.CustomerId == userId &&
							s.ShowId == showId &&
							s.LockedUntil > DateTime.UtcNow)
				.ToListAsync();
		}

		public async Task<List<BookingSeatLock>> GetLockedSeatsByShow(Guid showId)
		{
			var lockedSeats = await _context.BookingSeatLocks
				.Where(bl => bl.ShowId == showId &&
							 bl.LockedUntil > DateTime.UtcNow)
				.ToListAsync();

			return lockedSeats;
		}

		public async Task<bool> ExtendLockForPayment(Guid userId, Guid showId, List<Guid> seatIds)
		{
			var lockedSeats = await _context.BookingSeatLocks
				.Where(s => s.CustomerId == userId &&
							s.ShowId == showId &&
							seatIds.Contains(s.SeatId) && s.LockedUntil > DateTime.UtcNow)
				.ToListAsync();

			if (!lockedSeats.Any())
			{
				return false;
			}

			foreach (var seat in lockedSeats)
			{
				seat.IsPaymentInProgress = true;
				seat.LockedUntil = DateTime.UtcNow.AddMinutes(15);
			}

			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> ReleaseSeatsAfterSuccessfulPayment(Guid bookingId)
		{
			var bookedSeats = await _context.BookingSeats
				.Where(bs => bs.BookingId == bookingId)
				.Select(bs => bs.SeatId)
				.ToListAsync();

			if (!bookedSeats.Any())
			{
				return false;
			}

			var lockedSeats = await _context.BookingSeatLocks
				.Where(bl => bookedSeats.Contains(bl.SeatId))
				.ToListAsync();

			if (!lockedSeats.Any())
			{
				return false;
			}

			_context.BookingSeatLocks.RemoveRange(lockedSeats);
			await _context.SaveChangesAsync();

			return true;
		}
	}
}