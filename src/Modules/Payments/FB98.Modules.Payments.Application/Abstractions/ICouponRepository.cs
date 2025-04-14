using FB98.Modules.Payments.Domain.Entities;

namespace FB98.Modules.Payments.Application.Abstractions
{
	public interface ICouponRepository : IRepository<Coupon>
	{
		Task<bool> IsCouponExisted(string code);

		Task<Coupon?> GetValidCouponAsync(string code, decimal orderAmount, DateTime now);

		Task<bool> ApplyCouponAfterPaymentAsync(string code, Guid paymentTransactionId, decimal appliedAmount);
	}
}