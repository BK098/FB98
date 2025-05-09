using FB98.Shared.Abstractions.CQRS;
using FB98.Shared.Abstractions.Entities;
using FB98.Shared.Abstractions.Responses;
using FB98.Shared.Infrastructure.Paging;

namespace FB98.Modules.ShoppingList.Application.TodoManagement.GetAll
{
	public record GetAllTodoQuery(Filter Filter) : IQuery<ApiResult<PaginatedResult<GetAllTodoResponse>>>;
}