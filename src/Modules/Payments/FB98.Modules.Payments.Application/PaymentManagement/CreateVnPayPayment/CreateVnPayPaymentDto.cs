using Swashbuckle.AspNetCore.Annotations;

namespace FB98.Modules.Payments.Application.PaymentManagement.CreateVnPayPayment
{
	public class CreateVnPayPaymentDto
	{
		[SwaggerIgnore]
		public string? PhoneNumber { get; set; }
		[SwaggerIgnore]
		public string? Email { get; set; }
		[SwaggerIgnore]
		public Guid? UserId { get; set; }

		public Guid? OrderId { get; set; }
		public Guid? BookingId { get; set; }
		public string? CouponCode { get; set; }
	}
}