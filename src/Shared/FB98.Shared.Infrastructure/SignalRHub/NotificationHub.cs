using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace FB98.Shared.Infrastructure.SignalRHub
{
	public class NotificationHub : Hub
	{
		public override Task OnConnectedAsync()
		{
			var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			Console.WriteLine($"[Hub] Connected userId: {userId}"); // Kiểm tra xem có null không
			return base.OnConnectedAsync();
		}
		public async Task SendSeatUnlocks(Guid userId, Guid showId)
		{
			var message = $"Ghế của bạn tại suất chiếu {showId} đã hết thời gian giữ và được mở khóa.";
			await Clients.User(userId.ToString()).SendAsync("SendSeatUnlocks", message);
		}
	}
}