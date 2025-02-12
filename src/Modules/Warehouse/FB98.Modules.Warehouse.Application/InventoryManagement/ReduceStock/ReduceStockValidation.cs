namespace FB98.Modules.Warehouse.Application.InventoryManagement.ReduceStock
{
	public class ReduceStockValidation : AbstractValidator<ReduceStockDto>
	{
		public ReduceStockValidation(ILocalizedMessageService message)
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
