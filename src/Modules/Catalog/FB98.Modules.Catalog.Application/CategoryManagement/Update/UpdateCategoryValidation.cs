namespace FB98.Modules.Catalog.Application.CategoryManagement.Update
{
	public class UpdateCategoryValidation : AbstractValidator<UpdateCategoryDto>
	{
		public UpdateCategoryValidation(ILocalizedMessageService message)
		{
			RuleFor(x => x.Name)
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NameRequired"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NameEmpty"));
		}
	}
}
