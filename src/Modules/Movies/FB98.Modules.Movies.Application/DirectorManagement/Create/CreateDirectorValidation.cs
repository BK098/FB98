namespace FB98.Modules.Movies.Application.DirectorManagement.Create
{
	internal sealed class CreateDirectorValidation : AbstractValidator<CreateDirectorDto>
	{
		public CreateDirectorValidation(ILocalizedMessageService message)
		{
			RuleFor(x => x.Name)
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NameRequired"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NameEmpty"));
		}
	}
}