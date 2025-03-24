using FB98.Modules.Payments.Domain.Entities;

namespace FB98.Modules.Payments.Application.Abstractions
{
	public interface IPaymentRepository : IRepository<PaymentTransaction>
	{
		IQueryable<PaymentTransaction> GetAllPaymentByUser(Guid userId);
	}
}