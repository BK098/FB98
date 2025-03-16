using FB98.Modules.Identity.Application.Authentication.ForgotPassword;
using FB98.Modules.Identity.Application.Authentication.Login;
using FB98.Modules.Identity.Application.Authentication.Logout;
using FB98.Modules.Identity.Application.Authentication.RefreshToken;
using FB98.Modules.Identity.Application.Authentication.Register;
using FB98.Modules.Identity.Application.Authentication.ResetPassword;
using FB98.Modules.Identity.Application.Authentication.RevokeToken;
using FB98.Shared.Abstractions.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FB98.Modules.Identity.Api.Controllers
{
	internal class IdentityController : BaseController
	{
		public IdentityController(IMediator mediator) : base(mediator)
		{
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
					Secure = false,
					SameSite = SameSiteMode.Strict,
					Expires = DateTimeOffset.UtcNow.AddMinutes(15)
				});
				Response.Cookies.Append("refresh_token", result.Data!.RefreshToken, new CookieOptions
				{
					HttpOnly = true,
					Secure = false,
					SameSite = SameSiteMode.Strict,
					Expires = DateTimeOffset.UtcNow.AddDays(7)
				});
			}

			return StatusCode(result.StatusCode, result);
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterDto model)
		{
			var request = new RegisterCommand(model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
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
			}

			return StatusCode(result.StatusCode, result);
		}

		[HttpPost("forgot-password")]
		public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
		{
			var request = new ForgotPasswordCommand(model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[HttpPost("reset-password")]
		public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
		{
			var request = new ResetPasswordCommand(model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[HttpPost("refresh-token")]
		public async Task<IActionResult> RefreshToken()
		{
			var refreshToken = Request.Cookies["refresh_token"];
			if (refreshToken == null)
			{
				return StatusCode(400, ApiResponseBuilder.Error<string>("InvalidToken"));
			}

			var request = new RefreshTokenCommand(refreshToken);
			var result = await _mediator.Send(request);
			if (result.IsSuccess)
			{
				Response.Cookies.Append("access_token", result.Data!.Token, new CookieOptions
				{
					HttpOnly = true,
					Secure = false,
					SameSite = SameSiteMode.Strict,
					Expires = DateTimeOffset.UtcNow.AddMinutes(15)
				});
				Response.Cookies.Append("refresh_token", result.Data!.RefreshToken, new CookieOptions
				{
					HttpOnly = true,
					Secure = false,
					SameSite = SameSiteMode.Strict,
					Expires = DateTimeOffset.UtcNow.AddDays(7)
				});
			}

			return StatusCode(result.StatusCode, result);
		}

		[Authorize(Roles = "Administrator")]
		[HttpPost("revoke-token")]
		public async Task<IActionResult> RevokeToken(Guid userId)
		{
			var request = new RevokeTokenCommand(userId);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}
	}
}