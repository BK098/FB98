namespace FB98.Modules.Catalog.Application.CategoryManagement.Create
{
	internal sealed class CreateCategoryValidation : AbstractValidator<CreateCategoryDto>
	{
		public CreateCategoryValidation(ILocalizedMessageService message)
		{
			RuleFor(x => x.Name)
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NameRequired"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NameEmpty"));
		}
	}
}
