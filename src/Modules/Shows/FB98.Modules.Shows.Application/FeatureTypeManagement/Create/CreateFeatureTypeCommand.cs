namespace FB98.Modules.Shows.Application.FeatureTypeManagement.Create
{
	public record CreateFeatureTypeCommand(CreateFeatureTypeDto Model) : ICommand<ApiResult<object>>;
}