using FB98.Modules.ShoppingList.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.ShoppingList.DataAccess.Data
{
	public class ShoppingListModuleDbContext : DbContext
	{
		public ShoppingListModuleDbContext(DbContextOptions<ShoppingListModuleDbContext> options) : base(options)
		{
		}

		public DbSet<Todo> Todos { get; set; }
		public DbSet<TodoItem> TodoItems { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.HasDefaultSchema("ShoppingListModule");
			modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
		}
	}
}