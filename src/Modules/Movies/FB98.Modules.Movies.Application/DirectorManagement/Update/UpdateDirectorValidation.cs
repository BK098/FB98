namespace FB98.Modules.Movies.Application.DirectorManagement.Update
{
	internal sealed class UpdateDirectorValidation : AbstractValidator<UpdateDirectorDto>
	{
		public UpdateDirectorValidation(ILocalizedMessageService message)
		{
			RuleFor(x => x.Name)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));
		}
	}
}