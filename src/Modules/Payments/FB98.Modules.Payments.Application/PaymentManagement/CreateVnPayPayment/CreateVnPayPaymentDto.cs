using Swashbuckle.AspNetCore.Annotations;

namespace FB98.Modules.Payments.Application.PaymentManagement.CreateVnPayPayment
{
	public class CreateVnPayPaymentDto
	{
		[SwaggerIgnore]
		public Guid? UserId { get; set; }
		public Guid? OrderId { get; set; }
		public Guid? BookingId { get; set; }
	}
}