using FB98.Modules.Payments.Application.Abstractions;
using FB98.Modules.Payments.DataAccess.Data;
using FB98.Modules.Payments.Domain.Entities;
using FB98.Shared.Infrastructure.Repositpries;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Payments.DataAccess.Repositories
{
	public class CouponRepository : BaseRepository<Coupon, PaymentModuleDbContext>, ICouponRepository
	{
		public CouponRepository(PaymentModuleDbContext context) : base(context)
		{
		}

		public async Task<bool> IsCouponExisted(string code)
		{
			return await _context.Coupons.AnyAsync(x => x.Code == code);
		}

		public override async Task<bool> CreateAsync(Coupon entity)
		{
			entity.SetCreatedAt();
			await base.CreateAsync(entity);
			await _context.SaveChangesAsync();
			return true;
		}

		public override bool Update(Coupon entity)
		{
			entity.SetUpdatedAt();
			base.Update(entity);
			_context.SaveChanges();
			return true;
		}

		public override bool Delete(Coupon entity)
		{
			base.Delete(entity);
			_context.SaveChanges();
			return true;
		}

		public async Task<Coupon?> GetValidCouponAsync(string code, decimal orderAmount, DateTime now)
		{
			return await _context.Coupons
				.AsNoTracking()
				.Where(c => c.Code == code
							&& c.IsActive
							&& c.StartDate <= now
							&& c.EndDate >= now
							&& (!c.IsLimited || c.UsageCount < c.MaxUsage)
							&& orderAmount >= c.MinPaymentAmount)
				.FirstOrDefaultAsync();
		}

		public async Task<bool> ApplyCouponAfterPaymentAsync(
			string code,
			Guid paymentTransactionId,
			decimal appliedAmount)
		{
			var coupon = await _context.Coupons
				.Where(c => c.Code == code && c.IsActive)
				.FirstOrDefaultAsync();

			if (coupon == null)
			{
				return false;
			}

			if (coupon.IsLimited && coupon.UsageCount >= coupon.MaxUsage)
			{
				return false;
			}

			coupon.UsageCount += 1;
			coupon.SetUpdatedAt();

			var application = new CouponApplication
			{
				CouponId = coupon.Id,
				PaymentTransactionId = paymentTransactionId,
				AppliedAmount = appliedAmount
			};

			_context.CouponApplications.Add(application);

			try
			{
				await _context.SaveChangesAsync();
				return true;
			}
			catch (DbUpdateConcurrencyException)
			{
				// Có conflict vì nhiều người dùng cùng lúc
				return false;
			}
		}
	}
}