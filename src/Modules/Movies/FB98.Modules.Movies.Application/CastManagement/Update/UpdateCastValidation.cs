namespace FB98.Modules.Movies.Application.CastManagement.Update
{
	internal sealed class UpdateCastValidation : AbstractValidator<UpdateCastDto>
	{
		public UpdateCastValidation(ILocalizedMessageService message)
		{
			RuleFor(x => x.Name)
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NameRequired"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NameEmpty"));
		}
	}
}