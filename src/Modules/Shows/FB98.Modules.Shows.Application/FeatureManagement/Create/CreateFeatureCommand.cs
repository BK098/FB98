namespace FB98.Modules.Shows.Application.FeatureManagement.Create
{
	public record CreateFeatureCommand(CreateFeatureDto Model) : ICommand<ApiResult<object>>;
}