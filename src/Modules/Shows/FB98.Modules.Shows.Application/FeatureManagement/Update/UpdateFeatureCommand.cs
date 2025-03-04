namespace FB98.Modules.Shows.Application.FeatureManagement.Update
{
	public record UpdateFeatureCommand(Guid FeatureId, UpdateFeatureDto Model) : ICommand<ApiResult<object>>;
}