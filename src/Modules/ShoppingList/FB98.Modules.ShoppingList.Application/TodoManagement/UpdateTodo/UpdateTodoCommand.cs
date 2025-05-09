using FB98.Shared.Abstractions.CQRS;
using FB98.Shared.Abstractions.Responses;

namespace FB98.Modules.ShoppingList.Application.TodoManagement.UpdateTodo
{
	public record UpdateTodoCommand(Guid TodoId, UpdateTodoDto Model) : ICommand<ApiResult<object>>;
}