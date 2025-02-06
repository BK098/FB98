using FB98.Shared.Abstractions.CQRS;
using FB98.Shared.Abstractions.Responses;


namespace FB98.Modules.Identity.Application.DeviceManagement.RevokeDeviceToken
{
	public record RevokeDeviceTokenCommand(RevokeDeviceTokenDto Model) : ICommand<ApiResponse<object>>;
}