using Microsoft.AspNetCore.SignalR;

namespace FB98.Shared.Infrastructure.SignalRHub
{
	public class SeatHub : Hub
	{
		public async Task UpdateSeatsStatus(Guid showId)
		{
			await Clients.All.SendAsync("SeatsStatusChanged", showId);
		}
	}
}