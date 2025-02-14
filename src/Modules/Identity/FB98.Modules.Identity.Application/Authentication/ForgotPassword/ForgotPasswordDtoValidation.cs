namespace FB98.Modules.Identity.Application.Authentication.ForgotPassword
{
	internal sealed class ForgotPasswordDtoValidation : AbstractValidator<ForgotPasswordDto>
	{
		public ForgotPasswordDtoValidation(ILocalizedMessageService message)
		{
			RuleFor(x => x.Email)
				.Cascade(CascadeMode.Stop)
				.NotEmpty().WithMessage(message.GetLocalizedMessage("EmailRequired"))
				.EmailAddress().WithMessage(message.GetLocalizedMessage("EmailInvalid"));
		}
	}
}
