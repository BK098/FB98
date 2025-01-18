namespace FB98.Shared.Infrastructure.Email
{
	public interface IEmailSender
	{
		Task SendEmailAsync(string email, string subject, string message);
	}
}
