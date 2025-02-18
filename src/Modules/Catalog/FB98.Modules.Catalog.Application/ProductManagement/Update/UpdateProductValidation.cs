namespace FB98.Modules.Catalog.Application.ProductManagement.Update
{
	internal sealed class UpdateProductValidation : AbstractValidator<UpdateProductDto>
	{
		public UpdateProductValidation(ILocalizedMessageService message)
		{
			RuleFor(x => x.Name)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));

			RuleFor(x => x.Description)
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));

			RuleFor(x => x.ProductImage)
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));

			RuleFor(x => x.IsEnabled)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));

			RuleFor(x => x.Price)
				.GreaterThanOrEqualTo(0).WithMessage(message.GetLocalizedMessage("GreaterThanOrEqualTo0"))
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));

			RuleFor(x => x.Quantity)
				.GreaterThanOrEqualTo(0).WithMessage(message.GetLocalizedMessage("GreaterThanOrEqualTo0"))
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));
		}
	}
}
