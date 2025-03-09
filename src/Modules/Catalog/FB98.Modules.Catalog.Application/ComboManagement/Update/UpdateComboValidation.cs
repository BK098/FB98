namespace FB98.Modules.Catalog.Application.ComboManagement.Update
{
	internal sealed class UpdateComboValidation : AbstractValidator<UpdateComboDto>
	{
		public UpdateComboValidation(ILocalizedMessageService message)
		{
			RuleFor(x => x.Name)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));

			RuleFor(x => x.IsEnabled)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));

			RuleFor(x => x.Price)
				.GreaterThanOrEqualTo(0).WithMessage(message.GetLocalizedMessage("GreaterThanOrEqualTo0"))
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));

			RuleForEach(x => x.Products).SetValidator(new UpdateComboProductValidation(message));

			RuleFor(x => x.Products)
				.Must(BeUniqueProductIds).WithMessage(message.GetLocalizedMessage("DuplicateData"));
		}

		private bool BeUniqueProductIds(ICollection<UpdateComboProductDto>? products)
		{
			if (products == null)
			{
				return true;
			}

			var productIds = products.Where(s => s.ProductId.HasValue).Select(s => s.ProductId!.Value).ToList();
			return productIds.Distinct().Count() == productIds.Count();
		}
	}

	internal sealed class UpdateComboProductValidation : AbstractValidator<UpdateComboProductDto>
	{
		public UpdateComboProductValidation(ILocalizedMessageService message)
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