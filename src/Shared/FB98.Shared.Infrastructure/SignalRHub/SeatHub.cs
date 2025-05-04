using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace FB98.Shared.Infrastructure.SignalRHub
{
	public class SeatHub : Hub
	{
		private readonly ILogger<SeatHub> _logger;

		public SeatHub(ILogger<SeatHub> logger)
		{
			_logger = logger;
		}

		public override async Task OnConnectedAsync()
		{
			try
			{
				var httpContext = Context.GetHttpContext();
				var showId = httpContext?.Request.Query["showId"].ToString();

				_logger.LogInformation("SeatHub - New connection: {ConnectionId} - showId: {ShowId}", Context.ConnectionId, showId);

				if (!Guid.TryParse(showId, out var parsedShowId))
				{
					// Không hợp lệ → trả lỗi custom nếu muốn
					_logger.LogWarning("Invalid showId in query string: {Raw}", showId);
					Context.Abort(); // <- Có thể gây lỗi handshake
					return;
				}

				await Groups.AddToGroupAsync(Context.ConnectionId, parsedShowId.ToString());
				_logger.LogInformation("SeatHub - Added to group: {Group}", parsedShowId);
				await base.OnConnectedAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error in OnConnectedAsync");
				throw; // Gây ra lỗi handshake (tốt để log rõ ràng)
			}
		}

		public async Task UpdateSeatsStatus(Guid showId)
		{
			await Clients.All.SendAsync("SeatsStatusChanged", showId);
		}
	}
}