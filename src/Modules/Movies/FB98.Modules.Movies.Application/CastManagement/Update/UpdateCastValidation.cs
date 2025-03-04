namespace FB98.Modules.Movies.Application.CastManagement.Update
{
	internal sealed class UpdateCastValidation : AbstractValidator<UpdateCastDto>
	{
		public UpdateCastValidation(ILocalizedMessageService message)
		{
			RuleFor(x => x.Name)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));
		}
	}
}