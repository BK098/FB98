namespace FB98.Modules.Identity.Application.DeviceManagement.RevokeDeviceToken
{
	public record RevokeDeviceTokenCommand(RevokeDeviceTokenDto Model) : ICommand<ApiResult<object>>;
}