namespace FB98.Modules.Movies.Application.DirectorManagement.GetDetail
{
	public record GetDetailDirectorQuery(Guid DirectorId) : IQuery<ApiResult<GetDetailDirectorResponse>>;
}