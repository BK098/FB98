namespace FB98.Modules.Identity.Application.DeviceManagement.RevokeAllDeviceToken
{
	public record RevokeAllDeviceTokenCommand(Guid UserId) : ICommand<ApiResponse<object>>;
}
