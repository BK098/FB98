namespace FB98.Modules.Payments.Application.CouponManagement.Update
{
	internal sealed class UpdateCouponValidation : AbstractValidator<UpdateCouponDto>
	{
		private readonly TimeSpan _bufferTime = TimeSpan.FromSeconds(60);

		public UpdateCouponValidation(ILocalizedMessageService message)
		{
			RuleFor(x => x.Code)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"))
				.MaximumLength(50).WithMessage(message.GetLocalizedMessage("MaxLength").Replace("{Max}", "50"));

			RuleFor(x => x.IsDiscountPercentage)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));

			RuleFor(x => x.StartDate)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"))
				.GreaterThanOrEqualTo(DateTime.UtcNow.Subtract(_bufferTime))
				.WithMessage(message.GetLocalizedMessage("StartDateValidation"));

			RuleFor(x => x.EndDate)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"))
				.Must((dto, endDate) => endDate > dto.StartDate)
				.GreaterThanOrEqualTo(DateTime.UtcNow).WithMessage(message.GetLocalizedMessage("EndDateValidation"));

			RuleFor(x => x.Value)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));

			When(x => x.IsDiscountPercentage == true, () =>
			{
				RuleFor(x => x.Value)
					.InclusiveBetween(1, 100).WithMessage(message.GetLocalizedMessage("DiscountPercentageRange"));

				RuleFor(x => x.MaxDiscountAmount)
					.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
					.GreaterThan(0).WithMessage(message.GetLocalizedMessage("GreaterThan0"));
			});

			RuleFor(x => x.MinPaymentAmount)
				.GreaterThanOrEqualTo(0).When(x => x.MinPaymentAmount.HasValue);

			RuleFor(x => x.Value)
				.GreaterThanOrEqualTo(1000).When(x => !x.IsDiscountPercentage!.Value).WithMessage(message.GetLocalizedMessage("GreaterThanOrEqualTo1000"));

			RuleFor(x => x.IsLimited)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));

			RuleFor(x => x.MaxUsage)
				.GreaterThanOrEqualTo(1).When(x => x.IsLimited!.Value).WithMessage(message.GetLocalizedMessage("GreaterThanOrEqualTo1"));
		}
	}
}