using FB98.Shared.Abstractions.CQRS;
using FB98.Shared.Abstractions.Responses;

namespace FB98.Modules.ShoppingList.Application.TodoManagement.GetDetailTodoItem
{
	public record GetDetailTodoItemQuery(Guid TodoItemId) : IQuery<ApiResult<GetDetailTodoItemResponse>>;
}