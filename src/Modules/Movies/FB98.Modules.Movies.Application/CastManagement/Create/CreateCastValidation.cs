namespace FB98.Modules.Movies.Application.CastManagement.Create
{
	internal sealed class CreateCastValidation : AbstractValidator<CreateCastDto>
	{
		public CreateCastValidation(ILocalizedMessageService message)
		{
			RuleFor(x => x.Name)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));
		}
	}
}