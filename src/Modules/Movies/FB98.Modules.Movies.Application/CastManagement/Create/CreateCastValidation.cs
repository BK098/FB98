namespace FB98.Modules.Movies.Application.CastManagement.Create
{
	internal sealed class CreateCastValidation : AbstractValidator<CreateCastDto>
	{
		public CreateCastValidation(ILocalizedMessageService message)
		{
			RuleFor(x => x.Name)
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NameRequired"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NameEmpty"));
		}
	}
}