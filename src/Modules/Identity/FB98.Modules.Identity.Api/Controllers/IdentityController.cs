using FB98.Modules.Identity.Application.Models;
using FB98.Modules.Identity.Application.Services;
using Microsoft.AspNetCore.Mvc;

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
			var response = await _authenticationService.LoginAsync(model);
			if (response.IsSuccess)
			{
				return Ok(response);
			}
			else
			{
				return BadRequest(response);
			}
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterDto model)
		{
			var response = await _authenticationService.RegisterAsync(model);
			if (response.IsSuccess)
			{
				return Ok(response);
			}
			else
			{
				return BadRequest(response);
			}
		}

		//[HttpPost("refresh-token")]
		//public async Task<IActionResult> RefreshToken()
		//{
		//	if (Request.Cookies.TryGetValue("refresh_token", out var refreshToken))
		//	{
		//		// Kiểm tra refreshToken hợp lệ (ví dụ: kiểm tra trong cơ sở dữ liệu)
		//		var user = await _authenticationService.ValidateRefreshToken(refreshToken);
		//		if (user == null) return Unauthorized();

		//		// Tạo Access Token mới
		//		var newAccessToken = await _tokenService.GenerateAccessToken(user);

		//		// Cập nhật Access Token trong cookie
		//		Response.Cookies.Append("access_token", newAccessToken, new CookieOptions
		//		{
		//			HttpOnly = true,
		//			Secure = true,
		//			SameSite = SameSiteMode.Strict,
		//			Expires = DateTimeOffset.UtcNow.AddMinutes(15)
		//		});

		//		return Ok(new { message = "Token refreshed" });
		//	}

		//	return Unauthorized();
		//}

		//private void SetTokensInCookies(string accessToken)
		//{
		//	var refreshToken = _tokenService.GenerateRefreshToken();

		//	Response.Cookies.Append("access_token", accessToken, new CookieOptions
		//	{
		//		HttpOnly = true,
		//		Secure = true,
		//		SameSite = SameSiteMode.Strict,
		//		Expires = DateTimeOffset.UtcNow.AddMinutes(15)
		//	});

		//	Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
		//	{
		//		HttpOnly = true,
		//		Secure = true,
		//		SameSite = SameSiteMode.Strict,
		//		Expires = DateTimeOffset.UtcNow.AddDays(7) // Refresh Token có thời gian sống lâu hơn
		//	});
		//}
	}
}
