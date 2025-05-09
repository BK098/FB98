using FB98.Shared.Abstractions.CQRS;
using FB98.Shared.Abstractions.Responses;

namespace FB98.Modules.ShoppingList.Application.TodoManagement.GetDetail
{
	public record GetDetailTodoQuery(Guid TodoId) : IQuery<ApiResult<GetDetailTodoResponse>>;
}