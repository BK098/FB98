namespace FB98.Modules.Catalog.Application.CategoryManagement.Update
{
	internal sealed class UpdateCategoryValidation : AbstractValidator<UpdateCategoryDto>
	{
		public UpdateCategoryValidation(ILocalizedMessageService message)
		{
			RuleFor(x => x.Name)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));
		}
	}
}