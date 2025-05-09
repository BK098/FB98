using FB98.Shared.Abstractions.CQRS;
using FB98.Shared.Abstractions.Responses;

namespace FB98.Modules.ShoppingList.Application.TodoManagement.UpdateTodoItem
{
	public record UpdateTodoItemCommand(Guid TodoItemId, UpdateTodoItemDto Model) : ICommand<ApiResult<object>>;
}