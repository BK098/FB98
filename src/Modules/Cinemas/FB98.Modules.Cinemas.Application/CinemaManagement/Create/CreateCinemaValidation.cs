namespace FB98.Modules.Cinemas.Application.CinemaManagement.Create
{
	internal sealed class CreateCinemaValidation : AbstractValidator<CreateCinemaDto>
	{
		public CreateCinemaValidation(ILocalizedMessageService message)
		{
			RuleFor(x => x.Name)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));

			RuleFor(x => x.Address)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));
		}
	}
}