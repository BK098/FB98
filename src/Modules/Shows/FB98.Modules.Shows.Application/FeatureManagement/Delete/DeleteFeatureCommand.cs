namespace FB98.Modules.Shows.Application.FeatureManagement.Delete
{
	public record DeleteFeatureCommand(Guid FeatureId) : ICommand<ApiResult<object>>;
}