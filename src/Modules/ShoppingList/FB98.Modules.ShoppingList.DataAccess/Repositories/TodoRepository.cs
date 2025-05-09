using FB98.Modules.ShoppingList.Application.Abstractions;
using FB98.Modules.ShoppingList.DataAccess.Data;
using FB98.Modules.ShoppingList.Domain.Entites;
using FB98.Shared.Infrastructure.Repositpries;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.ShoppingList.DataAccess.Repositories
{
	internal class TodoRepository : BaseRepository<Todo, ShoppingListModuleDbContext>, ITodoRepository
	{
		public TodoRepository(ShoppingListModuleDbContext context) : base(context)
		{
		}

		public override Task<Todo?> GetByIdAsync(Guid? id)
		{
			return _context.Todos.Include(x => x.Items).FirstOrDefaultAsync(x => id == x.Id);
		}
		public Task<TodoItem?> GetTodoItemByIdAsync(Guid? id)
		{
			return _context.TodoItems.FirstOrDefaultAsync(x => id == x.Id);
		}

		public bool CreateTodoItem(TodoItem entity)
		{
			_context.TodoItems.Add(entity);
			return true;
		}

		public async Task<bool> IsTodoExistsAsync(string title, CancellationToken cancellationToken)
		{
			return await GetAll().AnyAsync(c => c.Title == title, cancellationToken);
		}

		public bool UpdateTodoItem(TodoItem entity)
		{
			_context.TodoItems.Update(entity);
			return true;
		}
	}
}