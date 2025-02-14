namespace FB98.Modules.Warehouse.Application.InventoryManagement.AddStock
{
	internal sealed class AddStockValidation : AbstractValidator<AddStockDto>
	{
		public AddStockValidation(ILocalizedMessageService message)
		{
			RuleFor(x => x.ProductId)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));

			RuleFor(x => x.Quantity)
				.GreaterThanOrEqualTo(0).WithMessage(message.GetLocalizedMessage("GreaterThanOrEqualTo0"))
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));
		}
	}
}
