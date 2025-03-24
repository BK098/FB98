using FB98.Modules.Identity.Application.ProfileManagement.ChangePassword;
using FB98.Modules.Identity.Application.ProfileManagement.EditProfile;
using FB98.Modules.Identity.Application.ProfileManagement.GetProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FB98.Modules.Identity.Api.Controllers
{
	internal class ProfileController : BaseController
	{
		public ProfileController(IMediator mediator) : base(mediator)
		{
		}

		[Authorize]
		[HttpPost("change-password")]
		public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId == null)
			{
				return Unauthorized(new { message = "User is not authorized" });
			}

			var request = new ChangePasswordCommand(userId, model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize(Roles = "Administrator")]
		[HttpPost("{userId:guid}")]
		public async Task<IActionResult> ChangePassword(Guid userId, [FromBody] EditProfileDto model)
		{
			var request = new EditProfileCommand(userId, model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> GetUser([FromQuery] Guid? userId)
		{
			var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

			if (userId == null || userId == currentUserId)
			{
				userId = currentUserId;
			}
			else if (!User.IsInRole("Administrator"))
			{
				return Forbid();
			}

			var request = new GetProfileQuery(userId.ToString()!);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}
	}
}