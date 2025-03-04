namespace FB98.Modules.Shows.Application.FeatureTypeManagement.GetDetail
{
	public record GetDetailFeatureTypeQuery(Guid FeatureTypeId) : IQuery<ApiResult<GetDetailFeatureTypeResponse>>;
}