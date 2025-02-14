namespace FB98.Modules.Identity.Application.Authentication.ResetPassword
{
	internal sealed class ResetPasswordDtoValidation : AbstractValidator<ResetPasswordDto>
	{
		public ResetPasswordDtoValidation(ILocalizedMessageService message)
		{
			RuleFor(x => x.Email)
				.Cascade(CascadeMode.Stop)
				.NotEmpty().WithMessage(message.GetLocalizedMessage("EmailRequired"))
				.EmailAddress().WithMessage(message.GetLocalizedMessage("EmailInvalid"));

			RuleFor(x => x.Password)
				.NotEmpty().WithMessage(message.GetLocalizedMessage("PasswordRequired"))
				.MinimumLength(6).WithMessage(message.GetLocalizedMessage("PasswordTooShort"))
				.Matches(@"[A-Z]").WithMessage(message.GetLocalizedMessage("PasswordMustContainUppercase"))
				.Matches(@"[0-9]").WithMessage(message.GetLocalizedMessage("PasswordMustContainNumber"))
				.Matches(@"[\W]").WithMessage(message.GetLocalizedMessage("PasswordMustContainSpecialCharacter"));
		}
	}
}