namespace FB98.Modules.Shows.Application.FeatureTypeManagement.Create
{
	internal sealed class CreateFeatureTypeValidation : AbstractValidator<CreateFeatureTypeDto>
	{
		public CreateFeatureTypeValidation(ILocalizedMessageService message)
		{
			RuleFor(x => x.Name)
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"))
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"));
		}
	}
}