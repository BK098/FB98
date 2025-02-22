using FB98.Modules.Payments.Application.Abstractions;
using FB98.Modules.Payments.DataAccess.Data;
using FB98.Modules.Payments.Domain.Entities;
using FB98.Shared.Infrastructure.Repositpries;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Payments.DataAccess.Repositories
{
	public class PaymentRepository : BaseRepository<PaymentTransaction, PaymentModuleDbContext>, IPaymentRepository
	{
		public PaymentRepository(PaymentModuleDbContext context) : base(context)
		{
		}

		public override async Task<bool> CreateAsync(PaymentTransaction entity)
		{
			entity.SetCreatedAt();
			await base.CreateAsync(entity);
			await _context.SaveChangesAsync();
			return true;
		}

		public override bool Update(PaymentTransaction entity)
		{
			entity.SetUpdatedAt();
			base.Update(entity);
			_context.SaveChanges();
			return true;
		}

		public override async Task<PaymentTransaction?> GetByIdAsync(Guid? id)
		{
			return await _context.PaymentTransactions
				.Include(x => x.PaymentStatus)
				.Include(x => x.PaymentMethod)
				.FirstOrDefaultAsync(p => p.OrderId == id);
		}
	}
}