namespace FB98.Modules.Identity.Application.ProfileManagement.ChangePassword
{
	internal sealed class ChangePasswordDtoValidation : AbstractValidator<ChangePasswordDto>
	{
		public ChangePasswordDtoValidation(ILocalizedMessageService message)
		{
			RuleFor(x => x.CurrentPassword)
				.NotEmpty().WithMessage(message.GetLocalizedMessage("CurrentPasswordRequired"));

			RuleFor(x => x.NewPassword)
				.NotEmpty().WithMessage(message.GetLocalizedMessage("PasswordRequired"))
				.MinimumLength(6).WithMessage(message.GetLocalizedMessage("PasswordTooShort"))
				.Matches(@"[A-Z]").WithMessage(message.GetLocalizedMessage("PasswordMustContainUppercase"))
				.Matches(@"[0-9]").WithMessage(message.GetLocalizedMessage("PasswordMustContainNumber"))
				.Matches(@"[^a-zA-Z0-9]").WithMessage(message.GetLocalizedMessage("PasswordMustContainSpecialCharacter"))
				.NotEqual(x => x.CurrentPassword).WithMessage(message.GetLocalizedMessage("PasswordMustBeDifferent"));
		}
	}
}
