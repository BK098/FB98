using FB98.Shared.Abstractions.Entities;
using FB98.Shared.Infrastructure.Paging;

namespace FB98.Modules.Cinemas.Application.CinemaManagement.GetAll
{
	public record GetAllCinemaQuery(Filter Filter) : IQuery<ApiResult<PaginatedResult<GetAllCinemaResponse>>>;
}