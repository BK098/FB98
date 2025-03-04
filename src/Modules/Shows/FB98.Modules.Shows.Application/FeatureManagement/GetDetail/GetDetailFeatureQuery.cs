namespace FB98.Modules.Shows.Application.FeatureManagement.GetDetail
{
	public record GetDetailFeatureQuery(Guid FeatureId) : IQuery<ApiResult<GetDetailFeatureResponse>>;
}