namespace FB98.Modules.Shows.Application.FeatureTypeManagement.Update
{
	public class UpdateFeatureValidation : AbstractValidator<UpdateFeatureTypeDto>
	{
		public UpdateFeatureValidation(ILocalizedMessageService message)
		{
			RuleFor(x => x.Name)
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"))
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"));
		}
	}
}