using System.Net;
using System.Net.Mail;

namespace FB98.Shared.Infrastructure.Email
{
	public class EmailSender : IEmailSender
	{
		private readonly string _smtpServer = "smtp.gmail.com";
		private readonly int _smtpPort = 587;
		private readonly string _fromEmail = "buikhang122004@gmail.com";
		public async Task SendEmailAsync(string email, string subject, string message)
		{
			try
			{
				MailMessage mailMessage = new MailMessage();
				mailMessage.From = new MailAddress(_fromEmail);
				mailMessage.Subject = subject;
				mailMessage.To.Add(new MailAddress(email));
				mailMessage.Body = message;
				mailMessage.IsBodyHtml = true;

				var stmpClient = new SmtpClient(_smtpServer)
				{
					Port = _smtpPort,
					Credentials = new NetworkCredential(_fromEmail, "wxjjleihszkxonqn"),
					EnableSsl = true
				};

				await stmpClient.SendMailAsync(mailMessage);

			}
			catch (Exception ex)
			{
				Console.WriteLine($@"Error sending email: {ex.Message}");
				throw;
			}
		}
	}
}
