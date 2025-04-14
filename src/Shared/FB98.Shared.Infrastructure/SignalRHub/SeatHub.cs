using Microsoft.AspNetCore.SignalR;

namespace FB98.Shared.Infrastructure.SignalRHub
{
	public class SeatHub : Hub
	{
		public override async Task OnConnectedAsync()
		{
			var showIdQuery = Context.GetHttpContext()?.Request.Query["showId"];
			if (Guid.TryParse(showIdQuery, out var showId))
			{
				await Groups.AddToGroupAsync(Context.ConnectionId, showId.ToString());
				await base.OnConnectedAsync();
			}
			else
			{
				Context.Abort();
			}
		}

		public async Task UpdateSeatsStatus(Guid showId)
		{
			await Clients.All.SendAsync("SeatsStatusChanged", showId);
		}
	}
}