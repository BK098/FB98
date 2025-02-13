using FB98.Shared.Abstractions.Entities;
using Microsoft.EntityFrameworkCore;

namespace FB98.Shared.Infrastructure.Repositpries
{
	public abstract class BaseUnitOfWork<TDbContext> : IBaseUnitOfWork where TDbContext : DbContext
	{
		protected readonly TDbContext _context;
		public BaseUnitOfWork(TDbContext context)
		{
			_context = context;
		}
		public void Entry<TEntity>(TEntity entity, EntityState state) where TEntity : class, IEntity
		{
			var entry = _context.Entry(entity);
			entry.State = state;
		}
		public async Task<int> SaveChangesAsync()
		{
			UpdateTimestamps();
			return await _context.SaveChangesAsync();
		}

		public void Dispose()
		{
			_context.Dispose();
		}

		private void UpdateTimestamps()
		{
			var entries = _context.ChangeTracker.Entries()
				.Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

			foreach (var entry in entries)
			{
				var entity = (BaseEntity)entry.Entity;

				if (entry.State == EntityState.Added)
				{
					entity.SetCreatedAt();
				}
				entity.SetUpdatedAt();
			}
		}
	}
}
