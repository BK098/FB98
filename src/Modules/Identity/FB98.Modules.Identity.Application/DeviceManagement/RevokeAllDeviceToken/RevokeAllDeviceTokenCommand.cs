using FB98.Shared.Abstractions.CQRS;
using FB98.Shared.Abstractions.Responses;

namespace FB98.Modules.Identity.Application.DeviceManagement.RevokeAllDeviceToken
{
	public record RevokeAllDeviceTokenCommand(Guid UserId) : ICommand<ApiResponse<object>>;
}
