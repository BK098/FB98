using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace FB98.Shared.Infrastructure.Email
{
	public class EmailSender : IEmailSender
	{
		private const string FromEmail = "buikhang122004@gmail.com";
		private const int SmtpPort = 587;
		private const string SmtpServer = "smtp.gmail.com";

		/// <summary>
		/// Gửi email thông thường
		/// </summary>
		/// <param name="email"></param>
		/// <param name="subject"></param>
		/// <param name="message"></param>
		/// <returns></returns>
		public async Task SendEmailAsync(string email, string subject, string message)
		{
			try
			{
				var mailMessage = new MailMessage();
				mailMessage.From = new MailAddress(FromEmail);
				mailMessage.Subject = subject;
				mailMessage.To.Add(new MailAddress(email));
				mailMessage.Body = message;
				mailMessage.IsBodyHtml = true;

				var stmpClient = new SmtpClient(SmtpServer)
				{
					Port = SmtpPort,
					Credentials = new NetworkCredential(FromEmail, "wxjjleihszkxonqn"),
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

		/// <summary>
		/// Gửi email với đính kèm file ảnh dưới dạng inline
		/// </summary>
		/// <param name="email"></param>
		/// <param name="subject"></param>
		/// <param name="htmlMessage"></param>
		/// <param name="attachmentData"></param>
		/// <param name="attachmentName"></param>
		/// <returns></returns>
		public async Task SendEmailWithInlineAttachmentAsync(string email, string subject, string htmlMessage, byte[] attachmentData, string attachmentName)
		{
			try
			{
				var mailMessage = new MailMessage();
				mailMessage.From = new MailAddress(FromEmail);
				mailMessage.To.Add(new MailAddress(email));
				mailMessage.Subject = subject;
				mailMessage.IsBodyHtml = true;

				var htmlView = AlternateView.CreateAlternateViewFromString(htmlMessage, null, "text/html");

				// Đính kèm QR dưới dạng inline
				var imageStream = new MemoryStream(attachmentData);
				var linkedResource = new LinkedResource(imageStream, "image/png")
				{
					ContentId = attachmentName,
					TransferEncoding = TransferEncoding.Base64
				};

				htmlView.LinkedResources.Add(linkedResource);
				mailMessage.AlternateViews.Add(htmlView);

				var smtpClient = new SmtpClient(SmtpServer, SmtpPort)
				{
					Credentials = new NetworkCredential(FromEmail, "wxjjleihszkxonqn"),
					EnableSsl = true
				};

				await smtpClient.SendMailAsync(mailMessage);
			}
			catch (Exception ex)
			{
				Console.WriteLine($@"Error sending email: {ex.Message}");
				throw;
			}
		}
		public async Task SendEmailWithMultipleInlineAttachmentsAsync(string email, string subject, string htmlMessage, List<(byte[] attachmentData, string attachmentName)> attachments)
		{
			try
			{
				var mailMessage = new MailMessage
				{
					From = new MailAddress(FromEmail),
					Subject = subject,
					IsBodyHtml = true
				};

				mailMessage.To.Add(new MailAddress(email));

				var htmlView = AlternateView.CreateAlternateViewFromString(htmlMessage, null, "text/html");

				foreach (var (attachmentData, attachmentName) in attachments)
				{
					var imageStream = new MemoryStream(attachmentData);
					var linkedResource = new LinkedResource(imageStream, "image/png")
					{
						ContentId = attachmentName,
						TransferEncoding = TransferEncoding.Base64
					};

					htmlView.LinkedResources.Add(linkedResource);
				}

				mailMessage.AlternateViews.Add(htmlView);

				var smtpClient = new SmtpClient(SmtpServer, SmtpPort)
				{
					Credentials = new NetworkCredential(FromEmail, "wxjjleihszkxonqn"),
					EnableSsl = true
				};

				await smtpClient.SendMailAsync(mailMessage);
			}
			catch (Exception ex)
			{
				Console.WriteLine($@"Error sending email: {ex.Message}");
				throw;
			}
		}

	}
}