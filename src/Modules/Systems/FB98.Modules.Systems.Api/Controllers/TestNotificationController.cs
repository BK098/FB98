using FB98.Shared.Infrastructure.SignalRHub;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace FB98.Modules.Systems.Api.Controllers
{
	internal class TestNotificationController : BaseController
	{
		private readonly IHubContext<NotificationHub> _hubContext;

		public TestNotificationController(IHubContext<NotificationHub> hubContext)
		{
			_hubContext = hubContext;
		}

		// Gửi test đến tất cả
		[HttpGet("broadcast")]
		public async Task<IActionResult> BroadcastMessage()
		{
			await _hubContext.Clients.All.SendAsync("SendSeatUnlocks", "Một số ghế đã được mở khóa.");
			return Ok(new { message = "Sent to all" });
		}

		// Gửi test đến user cụ thể
		[HttpGet("user/{userId:guid}")]
		public async Task<IActionResult> SendToUser(Guid userId)
		{
			await _hubContext.Clients.User(userId.ToString())
				.SendAsync("SendSeatUnlocks", $"🔓 Ghế bạn đang giữ đã hết thời gian và được mở khóa.");
			return Ok(new { message = $"Sent to user {userId}" });
		}
	}
}