using FB98.Modules.ShoppingList.Domain.Entites;
using FB98.Shared.Infrastructure.Repositpries;

namespace FB98.Modules.ShoppingList.Application.Abstractions
{
	public interface ITodoRepository : IRepository<Todo>
	{
		bool CreateTodoItem(TodoItem entity);
		Task<bool> IsTodoExistsAsync(string title, CancellationToken cancellationToken);
		Task<TodoItem?> GetTodoItemByIdAsync(Guid? id);
		bool UpdateTodoItem(TodoItem entity);
	}
}