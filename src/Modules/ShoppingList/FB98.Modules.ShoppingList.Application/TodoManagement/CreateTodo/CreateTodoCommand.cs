using FB98.Shared.Abstractions.CQRS;
using FB98.Shared.Abstractions.Responses;

namespace FB98.Modules.ShoppingList.Application.TodoManagement.CreateTodo
{
	public record CreateTodoCommand(CreateTodoDto Model) : ICommand<ApiResult<object>>;
}