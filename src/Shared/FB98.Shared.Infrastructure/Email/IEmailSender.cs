namespace FB98.Shared.Infrastructure.Email
{
	public interface IEmailSender
	{
		Task SendEmailAsync(string email, string subject, string message);
		Task SendEmailWithInlineAttachmentAsync(string email, string subject, string htmlMessage, byte[] attachmentData, string attachmentName);
		Task SendEmailWithMultipleInlineAttachmentsAsync(string email, string subject, string htmlMessage, List<(byte[] attachmentData, string attachmentName)> attachments);
	}
}