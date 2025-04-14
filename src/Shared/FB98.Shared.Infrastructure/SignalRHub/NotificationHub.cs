using Microsoft.AspNetCore.SignalR;

namespace FB98.Shared.Infrastructure.SignalRHub
{
	public class NotificationHub : Hub
	{
		public async Task SendSeatUnlocks(Guid userId, Guid showId)
		{
			var message = $"Ghế của bạn tại suất chiếu {showId} đã hết thời gian giữ và được mở khóa.";
			await Clients.User(userId.ToString()).SendAsync("SendSeatUnlocks", message);
		}
	}
}