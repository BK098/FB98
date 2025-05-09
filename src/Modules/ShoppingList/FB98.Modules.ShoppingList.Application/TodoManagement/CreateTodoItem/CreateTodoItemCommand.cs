using FB98.Shared.Abstractions.CQRS;
using FB98.Shared.Abstractions.Responses;

namespace FB98.Modules.ShoppingList.Application.TodoManagement.CreateTodoItem
{
	public record CreateTodoItemCommand(CreateTodoItemDto Model) : ICommand<ApiResult<object>>;
}