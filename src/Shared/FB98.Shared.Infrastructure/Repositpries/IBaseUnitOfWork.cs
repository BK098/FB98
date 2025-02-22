using FB98.Shared.Abstractions.Entities;
using Microsoft.EntityFrameworkCore;

namespace FB98.Shared.Infrastructure.Repositpries
{
	public interface IBaseUnitOfWork : IDisposable
	{
		Task<int> SaveChangesAsync();
		void Entry<TEntity>(TEntity entity, EntityState state) where TEntity : class, IEntity;
	}
}