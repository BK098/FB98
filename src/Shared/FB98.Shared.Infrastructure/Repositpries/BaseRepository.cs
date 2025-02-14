using FB98.Shared.Abstractions.Entities;
using Microsoft.EntityFrameworkCore;

namespace FB98.Shared.Infrastructure.Repositpries
{
	public abstract class BaseRepository<TEntity, TContext> : IRepository<TEntity>
		where TEntity : class, IEntity
		where TContext : DbContext
	{
		protected readonly TContext _context;
		protected readonly DbSet<TEntity> _dbSet;
		public BaseRepository(TContext context)
		{
			_context = context;
			_dbSet = _context.Set<TEntity>();
		}
		public virtual async Task<TEntity?> GetByIdAsync(Guid? id)
		{
			var entity = await _dbSet.FirstOrDefaultAsync(x => x.Id.Equals(id));
			return entity;
		}
		public virtual async Task<TEntity?> GetByIdNoTrackingAsync(Guid? id)
		{
			var entity = await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Id.Equals(id));
			return entity;
		}
		public virtual async Task<TEntity?> FindByIdAsync(Guid? id)
		{
			var entity = await _dbSet.FindAsync(id);
			return entity;
		}
		public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
		{
			return await _dbSet.ToListAsync();
		}
		public virtual IQueryable<TEntity> GetAll()
		{
			return _dbSet;
		}
		public virtual async Task<bool> CreateAsync(TEntity entity)
		{
			await _dbSet.AddAsync(entity);
			return true;
		}
		public virtual bool Update(TEntity entity)
		{
			_dbSet.Update(entity);
			return true;
		}
		public virtual bool Create(TEntity entity)
		{
			_dbSet.Add(entity);
			return true;
		}
		public virtual bool Delete(TEntity entity)
		{
			_dbSet.Remove(entity);
			return true;
		}
	}
}