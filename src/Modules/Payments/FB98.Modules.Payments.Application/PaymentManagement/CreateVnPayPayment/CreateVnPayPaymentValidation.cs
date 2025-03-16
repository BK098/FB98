namespace FB98.Modules.Payments.Application.PaymentManagement.CreateVnPayPayment
{
	internal sealed class CreateVnPayPaymentValidation : AbstractValidator<CreateVnPayPaymentDto>
	{
		public CreateVnPayPaymentValidation(ILocalizedMessageService message)
		{
			RuleFor(x => x)
				.Must(x => x.OrderId.HasValue || x.BookingId.HasValue)
				.WithMessage(message.GetLocalizedMessage("OrderOrBookingRequired"));
			RuleFor(x => x.UserId)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));
		}
	}
}