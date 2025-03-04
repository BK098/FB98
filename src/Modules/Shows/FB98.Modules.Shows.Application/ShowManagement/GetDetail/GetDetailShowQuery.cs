namespace FB98.Modules.Shows.Application.ShowManagement.GetDetail
{
	public record GetDetailShowQuery(Guid ShowId) : IQuery<ApiResult<GetDetailShowResponse>>;
}