namespace FB98.Modules.Payments.Application.PaymentManagement.ApplyCoupon
{
	internal sealed class ApplyCouponValidation : AbstractValidator<ApplyCouponDto>
	{
		public ApplyCouponValidation(ILocalizedMessageService message)
		{
			RuleFor(x => x.CouponCode)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));

			RuleFor(x => x.Amount)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"))
				.GreaterThanOrEqualTo(1000).WithMessage(message.GetLocalizedMessage("GreaterThanOrEqualTo1000"));
		}
	}
}