namespace FB98.Modules.Identity.Application.Authentication.Login
{
	internal sealed class LoginValidation : AbstractValidator<LoginDto>
	{
		public LoginValidation(ILocalizedMessageService message)
		{
			RuleFor(x => x.Email)
				.Cascade(CascadeMode.Stop)
				.NotEmpty().WithMessage(message.GetLocalizedMessage("EmailRequired"))
				.EmailAddress().WithMessage(message.GetLocalizedMessage("EmailInvalid"));

			RuleFor(x => x.Password)
				.Cascade(CascadeMode.Stop)
				.NotEmpty().WithMessage(message.GetLocalizedMessage("PasswordRequired"));
		}
	}
}
