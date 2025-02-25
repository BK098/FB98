namespace FB98.Modules.Cinemas.Application.CinemaManagement.Update
{
	internal sealed class UpdateCinemaValidation : AbstractValidator<UpdateCinemaDto>
	{
		public UpdateCinemaValidation(ILocalizedMessageService message)
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