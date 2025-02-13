namespace FB98.Modules.Warehouse.Application.InventoryManagement.CreateInventory
{
	public class CreateInventoryValidation : AbstractValidator<CreateInventoryDto>
	{
		public CreateInventoryValidation(ILocalizedMessageService message)
		{
			RuleFor(x => x.ProductId)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));

			RuleFor(x => x.InitialStock)
				.GreaterThanOrEqualTo(0).WithMessage(message.GetLocalizedMessage("GreaterThanOrEqualTo0"))
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));
		}
	}
}
