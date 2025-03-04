namespace FB98.Modules.Shows.Application.FeatureTypeManagement.Delete
{
	public record DeleteFeatureTypeCommand(Guid FeatureTypeId) : ICommand<ApiResult<object>>;
}