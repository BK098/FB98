namespace FB98.Modules.Orders.Application.OrderManagement.Create
{
	public sealed class CreateOrderValidation : AbstractValidator<CreateOrderDto>
	{
		public CreateOrderValidation(ILocalizedMessageService message)
		{
			RuleFor(x => x.Items)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));

			RuleForEach(x => x.Items).ChildRules(item =>
			{
				item.RuleFor(i => i.ProductId)
					.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
					.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));

				item.RuleFor(i => i.IsCombo)
					.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
					.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));

				item.RuleFor(i => i.Quantity)
					.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
					.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"))
					.InclusiveBetween(1, 10).WithMessage(message.GetLocalizedMessage("QuantityRange"));
			});
		}
	}
}