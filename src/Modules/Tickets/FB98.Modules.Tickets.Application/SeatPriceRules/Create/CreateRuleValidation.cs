namespace FB98.Modules.Tickets.Application.SeatPriceRules.Create
{
	internal sealed class CreateRuleValidation : AbstractValidator<CreateRuleDto>
	{
		public CreateRuleValidation(ILocalizedMessageService message)
		{
			//RuleFor(x => x.SeatTypeId)
			//	.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
			//	.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));

			//RuleFor(x => x.Name)
			//	.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
			//	.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));

			//RuleFor(x => x.Price)
			//	.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
			//	.GreaterThan(0).WithMessage(message.GetLocalizedMessage("GreaterThanZero"));

			//RuleFor(x => x.StartDate)
			//	.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
			//	.LessThan(x => x.EndDate).WithMessage(message.GetLocalizedMessage("StartDateBeforeEndDate"));

			//RuleFor(x => x.EndDate)
			//	.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"));

			//RuleFor(x => x.MinAge)
			//	.GreaterThanOrEqualTo(0).WithMessage(message.GetLocalizedMessage("GreaterThanOrEqualToZero"));

			//RuleFor(x => x.MaxAge)
			//	.GreaterThan(x => x.MinAge).WithMessage(message.GetLocalizedMessage("MaxAgeGreaterThanMinAge"));

			//RuleFor(x => x.IsDefault)
			//	.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"));

			//RuleFor(x => x.IsActived)
			//	.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"));
		}
	}
}