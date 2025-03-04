namespace FB98.Modules.Shows.Application.FeatureTypeManagement.Update
{
	public record UpdateFeatureTypeCommand(Guid FeatureTypeId, UpdateFeatureTypeDto Model) : ICommand<ApiResult<object>>;
}