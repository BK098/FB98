using FB98.Modules.Identity.Application.Models;
using FB98.Modules.Identity.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FB98.Modules.Identity.Api.Controllers
{
	internal class IdentityController : BaseController
	{
		private readonly IAuthenticationService _authenticationService;
		public IdentityController(IAuthenticationService authenticationService)
		{
			_authenticationService = authenticationService;
		}
		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginDto model)
		{
			var result = await _authenticationService.LoginAsync(model);
			if (!result.IsSuccess)
			{
				return BadRequest(result);
			}
			return Ok(result);

		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterDto model)
		{
			var result = await _authenticationService.RegisterAsync(model);
			if (!result.IsSuccess)
			{
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpPost("forgot-password")]
		public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
		{
			var result = await _authenticationService.ForgotPasswordAsync(model);
			if (!result.IsSuccess)
			{
				return BadRequest(result);
			}
			return Ok(result);
		}
		[HttpPost("reset-password")]
		public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
		{
			var result = await _authenticationService.ResetPasswordAsync(model);
			if (!result.IsSuccess)
			{
				return BadRequest(result);
			}
			return Ok(result);
		}

		[Authorize]
		[HttpPost("change-password")]
		public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
		{
			// Lấy user ID từ token
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId == null)
			{
				return Unauthorized(new { message = "User is not authorized" });
			}

			var result = await _authenticationService.ChangePasswordAsync(userId, model);
			if (!result.IsSuccess)
			{
				return BadRequest(result);
			}

			return Ok(result);
		}
	}
}
