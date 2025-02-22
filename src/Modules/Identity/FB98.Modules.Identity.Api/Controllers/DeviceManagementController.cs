using FB98.Modules.Identity.Application.DeviceManagement.RevokeAllDeviceToken;
using FB98.Modules.Identity.Application.DeviceManagement.RevokeDeviceToken;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FB98.Modules.Identity.Api.Controllers
{
	internal class DeviceManagementController : BaseController
	{
		public DeviceManagementController(IMediator mediator) : base(mediator)
		{
		}

		[Authorize]
		[HttpPost("revoke-device")]
		public async Task<IActionResult> RevokeDeviceToken(Guid deviceId)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId == null)
			{
				return Unauthorized(new { message = "User is not authorized" });
			}

			var request = new RevokeDeviceTokenCommand(new RevokeDeviceTokenDto
			{
				UserId = Guid.Parse(userId),
				DeviceId = deviceId
			});
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize]
		[HttpPost("revoke-all-device")]
		public async Task<IActionResult> RevokeAllDeviceToken()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId == null)
			{
				return Unauthorized(new { message = "User is not authorized" });
			}

			var UserId = Guid.Parse(userId);
			var request = new RevokeAllDeviceTokenCommand(UserId);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}
	}
}