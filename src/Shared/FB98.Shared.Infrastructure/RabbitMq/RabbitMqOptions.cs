namespace FB98.Shared.Infrastructure.RabbitMq
{
	public class RabbitMqOptions
	{
		public string HostName { get; set; } = default!;
		public string UserName { get; set; } = default!;
		public string Password { get; set; } = default!;
		public string VirtualHost { get; set; } = "/";
		public int Port { get; set; }
	}
}