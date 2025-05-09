namespace FB98.Modules.Payments.Application.PaymentManagement.CreateVnPayPayment
{
	internal sealed class CreateVnPayPaymentValidation : AbstractValidator<CreateVnPayPaymentDto>
	{
		public CreateVnPayPaymentValidation(ILocalizedMessageService message)
		{
			RuleFor(x => x.UserId)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));
			RuleFor(x => x.OrderId)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));
		}
	}
}