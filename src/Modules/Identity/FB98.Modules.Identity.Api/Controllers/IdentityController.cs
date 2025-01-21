using FB98.Modules.Identity.Application.Authentication.ForgotPassword;
using FB98.Modules.Identity.Application.Authentication.Login;
using FB98.Modules.Identity.Application.Authentication.Logout;
using FB98.Modules.Identity.Application.Authentication.RefreshToken;
using FB98.Modules.Identity.Application.Authentication.Register;
using FB98.Modules.Identity.Application.Authentication.ResetPassword;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using FB98.Modules.Identity.Application.Authentication.RevokeToken;

namespace FB98.Modules.Identity.Api.Controllers
{
	internal class IdentityController : BaseController
	{
		private readonly IMediator _mediator;

		public IdentityController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginDto model)
		{
			var request = new LoginCommand(model);
			var result = await _mediator.Send(request);
			if (result.IsSuccess)
			{
				Response.Cookies.Append("access_token", result.Data!.Token, new CookieOptions
				{
					HttpOnly = true,
					Secure = true,
					SameSite = SameSiteMode.Strict,
					Expires = DateTimeOffset.UtcNow.AddMinutes(30)
				});
				Response.Cookies.Append("refresh_token", result.Data!.Token, new CookieOptions
				{
					HttpOnly = true,
					Secure = true,
					SameSite = SameSiteMode.Strict,
					Expires = DateTimeOffset.UtcNow.AddDays(7)
				});
				return Ok(result);
			}
			return BadRequest(result);
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterDto model)
		{
			var request = new RegisterCommand(model);
			var result = await _mediator.Send(request);
			if (!result.IsSuccess)
			{
				return BadRequest(result);
			}
			return Ok(result);
		}

		[Authorize]
		[HttpPost("logout")]
		public async Task<IActionResult> Logout()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId == null)
			{
				return Unauthorized(new { message = "User is not authorized" });
			}
			var request = new LogoutCommand(userId);
			var result = await _mediator.Send(request);
			if (result.IsSuccess)
			{
				Response.Cookies.Delete("access_token");
				Response.Cookies.Delete("refresh_token");
				return Ok(result);
			}
			return BadRequest(result);
		}

		[HttpPost("forgot-password")]
		public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
		{
			var request = new ForgotPasswordCommand(model);
			var result = await _mediator.Send(request);
			if (!result.IsSuccess)
			{
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpPost("reset-password")]
		public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
		{
			var request = new ResetPasswordCommand(model);
			var result = await _mediator.Send(request);
			if (!result.IsSuccess)
			{
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpPost]
		public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto model)
		{
			var request = new RefreshTokenCommand(model);
			var result = await _mediator.Send(request);
			if (result.IsSuccess)
			{
				Response.Cookies.Append("access_token", result.Data!.Token, new CookieOptions
				{
					HttpOnly = true,
					Secure = true,
					SameSite = SameSiteMode.Strict,
					Expires = DateTimeOffset.UtcNow.AddMinutes(30)
				});
				return Ok(result);
			}
			return BadRequest(result);
		}
		[Authorize]
		[HttpPost("revoke-token")]
		public async Task<IActionResult> RevokeToken()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId == null)
			{
				return Unauthorized(new { message = "User is not authorized" });
			}
			var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (currentUserId != userId.ToString() && !User.IsInRole("Admin"))
			{
				return Forbid();
			}

			var request = new RevokeTokenCommand(userId);
			var result = await _mediator.Send(request);
			if (!result.IsSuccess)
			{
				return BadRequest(result);
			}
			return Ok(result);
		}
	}
}
