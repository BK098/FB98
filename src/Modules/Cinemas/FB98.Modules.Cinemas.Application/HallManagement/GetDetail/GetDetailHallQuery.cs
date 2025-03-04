namespace FB98.Modules.Cinemas.Application.HallManagement.GetDetail
{
	public record GetDetailHallQuery(Guid HallId, bool IsSeatRange) : IQuery<ApiResult<GetDetailHallResponse>>;
}