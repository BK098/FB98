namespace FB98.Modules.Shows.Application.FeatureManagement.Create
{
	internal sealed class CreateFeatureValidation : AbstractValidator<CreateFeatureDto>
	{
		public CreateFeatureValidation(ILocalizedMessageService message)
		{
			RuleFor(x => x.Name)
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"))
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"));
			RuleFor(x => x.Description)
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"))
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"));
			RuleFor(x => x.FeatureTypeId)
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"))
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"));
		}
	}
}