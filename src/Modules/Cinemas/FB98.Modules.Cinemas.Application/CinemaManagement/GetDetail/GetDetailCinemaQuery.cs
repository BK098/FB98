namespace FB98.Modules.Cinemas.Application.CinemaManagement.GetDetail
{
	public record GetDetailCinemaQuery(Guid CinemaId) : IQuery<ApiResult<GetDetailCinemaResponse>>;
}