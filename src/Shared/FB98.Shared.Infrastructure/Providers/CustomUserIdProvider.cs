using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace FB98.Shared.Infrastructure.Providers
{
	public class CustomUserIdProvider : IUserIdProvider
	{
		public string? GetUserId(HubConnectionContext connection)
		{
			var userId = connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			Console.WriteLine(@"[SignalR] Connected UserId: {UserId}", userId);

			return userId;
		}
	}
}