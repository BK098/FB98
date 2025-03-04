namespace FB98.Modules.Movies.Application.DirectorManagement.Create
{
	internal sealed class CreateDirectorValidation : AbstractValidator<CreateDirectorDto>
	{
		public CreateDirectorValidation(ILocalizedMessageService message)
		{
			RuleFor(x => x.Name)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));
		}
	}
}