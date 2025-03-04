namespace FB98.Modules.Catalog.Application.DiscountManagement.CreateDiscountRule
{
	internal sealed class CreateDiscountRuleValidation : AbstractValidator<CreateDiscountRuleDto>
	{
		private readonly TimeSpan _bufferTime = TimeSpan.FromSeconds(60);

		public CreateDiscountRuleValidation(ILocalizedMessageService message)
		{
			RuleFor(x => x.Name)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));

			RuleFor(x => x.Description)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));

			RuleFor(x => x.IsDiscountPercentage)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));

			RuleFor(x => x.Value)
				.InclusiveBetween(0, 100).When(x => x.IsDiscountPercentage!.Value).WithMessage(message.GetLocalizedMessage("DiscountPercentageRange"));

			RuleFor(x => x.Value)
				.GreaterThanOrEqualTo(1000).When(x => !x.IsDiscountPercentage!.Value).WithMessage(message.GetLocalizedMessage("GreaterThanOrEqualTo1000"));

			RuleFor(x => x.StartDate)
				.GreaterThanOrEqualTo(DateTime.UtcNow.Subtract(_bufferTime))
				.WithMessage("Thời gian bắt đầu phải từ hiện tại trở đi.");

			RuleFor(x => x.EndDate)
				.GreaterThan(x => x.StartDate.AddMinutes(5)).WithMessage("Thời gian kết thúc phải lớn hơn thời gian bắt đầu ít nhất 5 phút.")
				.GreaterThanOrEqualTo(DateTime.UtcNow).WithMessage("Thời gian kết thúc không được nhỏ hơn thời gian hiện tại.");
		}
	}
}