namespace FB98.Modules.Movies.Application.DirectorManagement.Update
{
	internal sealed class UpdateDirectorValidation : AbstractValidator<UpdateDirectorDto>
	{
		public UpdateDirectorValidation(ILocalizedMessageService message)
		{
			RuleFor(x => x.Name)
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NameRequired"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NameEmpty"));
		}
	}
}