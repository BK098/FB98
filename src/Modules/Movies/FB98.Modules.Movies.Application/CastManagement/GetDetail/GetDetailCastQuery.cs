namespace FB98.Modules.Movies.Application.CastManagement.GetDetail
{
	public record GetDetailCastQuery(Guid CastId) : IQuery<ApiResult<GetDetailCastResponse>>;
}