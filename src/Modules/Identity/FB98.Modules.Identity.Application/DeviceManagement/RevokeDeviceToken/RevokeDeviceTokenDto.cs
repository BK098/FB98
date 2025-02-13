namespace FB98.Modules.Identity.Application.DeviceManagement.RevokeDeviceToken
{
	public class RevokeDeviceTokenDto
	{
		public Guid UserId { get; set; }
		public Guid DeviceId { get; set; }
	}
}