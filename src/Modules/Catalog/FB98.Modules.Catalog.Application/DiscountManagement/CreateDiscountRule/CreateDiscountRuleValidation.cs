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
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));

			RuleFor(x => x.Value)
				.InclusiveBetween(1, 100).When(x => x.IsDiscountPercentage!.Value).WithMessage(message.GetLocalizedMessage("DiscountPercentageRange"));

			RuleFor(x => x.Value)
				.GreaterThanOrEqualTo(1000).When(x => !x.IsDiscountPercentage!.Value).WithMessage(message.GetLocalizedMessage("GreaterThanOrEqualTo1000"));

			RuleFor(x => x.StartDate)
				.GreaterThanOrEqualTo(DateTime.UtcNow.Subtract(_bufferTime))
				.WithMessage(message.GetLocalizedMessage("StartDateValidation"));

			RuleFor(x => x.EndDate)
				.GreaterThan(x => x.StartDate.AddMinutes(5)).WithMessage(message.GetLocalizedMessage("EndDateValidation"))
				.GreaterThanOrEqualTo(DateTime.UtcNow).WithMessage(message.GetLocalizedMessage("EndDateValidation"));
		}
	}
}